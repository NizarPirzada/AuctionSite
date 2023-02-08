using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SuiteShared.DAL.Models;
using AuctionSite.DAL;
using Microsoft.EntityFrameworkCore;

namespace AuctionSite.Controllers
{
    public class ScrapperController : Controller
    {
        public AuctionContext dbContext = new AuctionContext();

        public IActionResult Index()
        {
            var aucData = (from p in dbContext.Auctions select p).ToList();

            return View(aucData);
        }
    }
}