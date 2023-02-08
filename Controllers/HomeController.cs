using System;
using System.Diagnostics;
using AuctionSite.DAL;
using AuctionSite.DAL.HelperObjects;
using AuctionSite.DAL.Models;
using AuctionSite.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AuctionSite.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private repo repo;

        public HomeController(repo repo) => this.repo = repo;

        public IActionResult Index() => View();


        [HttpGet]
        public IActionResult QueryEmail(string subject, string sender, string body)
        {
            SMTPHelper smtpObj = new SMTPHelper();
            
            try
            {
                smtpObj.SendQryEmail(sender, subject, body);
                return Json("Email Sent");
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        public IActionResult IncomeDisclosure()
        {
            return View();
        }
        
        public IActionResult GDPR_Policy()
        {
            return View();
        }
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
