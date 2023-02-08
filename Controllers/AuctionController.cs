using System;
using AuctionSite.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionSite.Controllers
{
    //this is the members area
    //if not logged in, should be sent back to home!

    [RequireHttps]
    [Authorize(Policy = "paid")]
    public class AuctionController : Controller
    {
        private repo repo;

        public AuctionController(repo repo) => this.repo = repo;

        public IActionResult Index(Guid id) => View(repo.GetAuctionInformation(id));

        public IActionResult IndexPartial(Guid id) => PartialView("_AuctionPartial", repo.GetAuctionInformation(id));
    }
}
