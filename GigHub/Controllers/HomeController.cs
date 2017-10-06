using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GigHub.Models;
using System.Data.Entity;
using GigHub.ViewModels;

namespace GigHub.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;

        public HomeController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var upcomingGigs = _context.Gigs
                .Include(x => x.Artist)
                .Include(x => x.Genre)
                .Where(x => x.DateTime > DateTime.Now);

            var homeViewModel = new GigsViewModel
            {
                UpcomingGigs = upcomingGigs, 
                ShowActions = User.Identity.IsAuthenticated,
                Heading = "Upcoming Gigs"
            };
            return View("Gigs", homeViewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}