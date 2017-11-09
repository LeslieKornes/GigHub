using GigHub.Persistence;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;
using GigHub.Core;
using GigHub.Core.Models;
using GigHub.Core.ViewModels;

namespace GigHub.Controllers
{
    public class GigsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public GigsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public ActionResult Mine()
        {
            var userId = User.Identity.GetUserId();
            var gigs = _unitOfWork.Gigs.GetUpcomingGigsByArtist(userId);

            return View(gigs);
        }

        [Authorize]
        public ActionResult Attending()
        {
            var userId = User.Identity.GetUserId();

            var gigsViewModel = new GigsViewModel
            {
                UpcomingGigs = _unitOfWork.Gigs.GetGigsUserAttending(userId),
                ShowActions = User.Identity.IsAuthenticated,
                Heading = "Gigs I'm Attending",
                Attendances = _unitOfWork.Attendances.GetFutureAttendances(userId).ToLookup(a => a.GigId)
            };

            return View("Gigs", gigsViewModel);
        }

        [HttpPost]
        public ActionResult Search(GigsViewModel gigsViewModel)
        {
            return RedirectToAction("Index", "Home", new {query = gigsViewModel.SearchTerm});
        }

        [Authorize]
        public ActionResult Create()
        {
            var gigFormViewModel = new GigFormViewModel
            {
                Genres = _unitOfWork.Genres.GetGenres(),
                Heading = "Add a Gig"
            };
            return View("GigForm", gigFormViewModel);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            var gig = _unitOfWork.Gigs.GetGig(id);

            if (gig == null)
                return HttpNotFound();

            if (gig.ArtistId != User.Identity.GetUserId())
                return new HttpUnauthorizedResult();

            var gigFormViewModel = new GigFormViewModel
            {
                Genres = new ApplicationDbContext().Genres.ToList(),
                Date = gig.DateTime.ToString("d MMM yyyy"),
                Time = gig.DateTime.ToString("HH:mm"),
                Genre = gig.GenreId,
                Venue = gig.Venue,
                Heading = "Edit a Gig",
                Id = gig.Id
            };
            return View("GigForm", gigFormViewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GigFormViewModel gigFormViewModel)
        {
            if (!ModelState.IsValid)
            {
                gigFormViewModel.Genres = new ApplicationDbContext().Genres.ToList();
                return View("GigForm", gigFormViewModel);
            }

            var gig = new Gig
            {
                ArtistId = User.Identity.GetUserId(),
                DateTime = gigFormViewModel.GetDateTime(),
                GenreId = gigFormViewModel.Genre,
                Venue = gigFormViewModel.Venue
            };

            _unitOfWork.Gigs.Add(gig);
            _unitOfWork.Complete();

            return RedirectToAction("Mine", "Gigs");
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(GigFormViewModel gigFormViewModel)
        {
            if (!ModelState.IsValid)
            {
                gigFormViewModel.Genres = new ApplicationDbContext().Genres.ToList();
                return View("GigForm", gigFormViewModel);
            }

            var gig = _unitOfWork.Gigs.GetGigWithAttendees(gigFormViewModel.Id);

            if (gig == null)
                return HttpNotFound();

            if (gig.ArtistId != User.Identity.GetUserId())
                return new HttpUnauthorizedResult();

            gig.Modify(gigFormViewModel.GetDateTime(), gigFormViewModel.Venue, gigFormViewModel.Genre);

            _unitOfWork.Complete();

            return RedirectToAction("Mine", "Gigs");
        }

        public ActionResult Details(int id)
        {
            var gig = _unitOfWork.Gigs.GetGig(id);

            if (gig == null)
            {
                return HttpNotFound();
            }

            var gigDetailsViewModel = new GigDetailsViewModel { Gig = gig };

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();

                gigDetailsViewModel.IsAttending = _unitOfWork.Attendances.GetAttendance(gig.Id, userId) != null;

                gigDetailsViewModel.IsFollowing = _unitOfWork.Followings.GetFollowing(gig.ArtistId, userId) != null;
            }

            return View("Details", gigDetailsViewModel);
        }

    }

   
}