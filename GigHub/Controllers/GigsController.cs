using GigHub.Models;
using GigHub.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace GigHub.Controllers
{
    public class GigsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GigsController()
        {
            _context = new ApplicationDbContext();
        }

        [Authorize]
        public ActionResult Mine()
        {
            var userId = User.Identity.GetUserId();
            var gigs = _context.Gigs
                .Where(g => 
                g.ArtistId == userId && 
                g.DateTime > DateTime.Now && 
                !g.IsCanceled)
                .Include(g => g.Genre)
                .ToList();

            return View(gigs);
        }

        [Authorize]
        public ActionResult Attending()
        {
            var userId = User.Identity.GetUserId();
            var gigs = _context.Attendances
                .Where(x => x.AttendeeId == userId)
                .Select(x => x.Gig)
                .Include(x => x.Artist)
                .Include(x => x.Genre)
                .ToList();

            var gigsViewModel = new GigsViewModel
            {
                UpcomingGigs = gigs,
                ShowActions = User.Identity.IsAuthenticated,
                Heading = "Gigs I'm Attending"
            };

            return View("Gigs", gigsViewModel);
        }

        [Authorize]
        public ActionResult Create()
        {
            var gigFormViewModel = new GigFormViewModel
            {
                Genres = _context.Genres.ToList(),
                Heading = "Add a Gig"
            };
            return View("GigForm", gigFormViewModel);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            var userId = User.Identity.GetUserId();
            var gig = _context.Gigs.Single(g => g.Id == id && g.ArtistId == userId);

            var gigFormViewModel = new GigFormViewModel
            {
                Genres = _context.Genres.ToList(),
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
                gigFormViewModel.Genres = _context.Genres.ToList();
                return View("GigForm", gigFormViewModel);
            }

            var gig = new Gig
            {
                ArtistId = User.Identity.GetUserId(),
                DateTime = gigFormViewModel.GetDateTime(),
                GenreId = gigFormViewModel.Genre,
                Venue = gigFormViewModel.Venue
            };

            _context.Gigs.Add(gig);
            _context.SaveChanges();

            return RedirectToAction("Mine", "Gigs");
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(GigFormViewModel gigFormViewModel)
        {
            var userId = User.Identity.GetUserId();

            if (!ModelState.IsValid)
            {
                gigFormViewModel.Genres = _context.Genres.ToList();
                return View("GigForm", gigFormViewModel);
            }

            var gig = _context.Gigs
                .Single(g => g.Id == gigFormViewModel.Id && g.ArtistId == userId);

            gig.Venue = gigFormViewModel.Venue;
            gig.DateTime = gigFormViewModel.GetDateTime();
            gig.GenreId = gigFormViewModel.Genre;

            _context.SaveChanges();

            return RedirectToAction("Mine", "Gigs");
        }
    }
}