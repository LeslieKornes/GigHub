using GigHub.Models;
using GigHub.ViewModels;
using Microsoft.AspNet.Identity;
using System;
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
        public ActionResult Create()
        {
            var gigFormViewModel = new GigFormViewModel
            {
                Genres = _context.Genres.ToList()
            };
            return View(gigFormViewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(GigFormViewModel gigFormViewModel)
        {
            var gig = new Gig
            {
                ArtistId = User.Identity.GetUserId(),
                DateTime = DateTime.Parse($"{gigFormViewModel.Date} {gigFormViewModel.Time}"),
                GenreId = gigFormViewModel.Genre,
                Venue = gigFormViewModel.Venue
            };

            _context.Gigs.Add(gig);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}