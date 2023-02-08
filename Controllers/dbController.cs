using AuctionSite.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AuctionSite.Controllers
{
    public class dbController : Controller
    {
        private readonly AuctionContext dbcontext;

        public dbController(AuctionContext dbcontext) => this.dbcontext = dbcontext;

        [Authorize(Roles = "admin")]
        public IActionResult index()
        {
            ViewBag.pendingMigrations = dbcontext.Database.GetPendingMigrations();
            ViewBag.appliedMigrations = dbcontext.Database.GetAppliedMigrations();
            ViewBag.getMigrations = dbcontext.Database.GetMigrations();
            ViewBag.auctionCount = dbcontext.Auctions.Count();
            ViewBag.propertydocumentsCount = dbcontext.PropertyDocument.Count();
            return View();
        }
    }
}
