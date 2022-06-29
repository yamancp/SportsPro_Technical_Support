using Microsoft.AspNetCore.Mvc;

//Author: Chad Paulsen
//Date Completed: 4/20/2022
//Class: Spring 2022 Developer Capstone

namespace SportsPro.Controllers
{
    public class HomeController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("About")]
        public IActionResult About()
        {
            return View();
        }
    }
}