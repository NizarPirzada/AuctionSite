using System;
using AuctionSite.DAL;
using AuctionSite.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionSite.DAL.Identity;
using AuctionSite.Helpers;
using Microsoft.AspNetCore.Identity;
using SuiteShared.DAL.Models;

namespace AuctionSite.Controllers
{
    //[Authorize(Policy = "paid")]
    [RequireHttps]
    public class AuctionsController : Controller
    {
        private readonly repo repo;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AuctionUser> userManager;
        private readonly SubscriptionHelper SubHelper;
        private readonly SignInManager<AuctionUser> SignInManager;
        AuctionContext dbcontext = new AuctionContext();

        //List<Auction> filteredAuctionsWithoutStateRestriction = new List<Auction>();
        public AuctionsController(repo repo,
            RoleManager<IdentityRole> RoleManager,
            UserManager<AuctionUser> UserManager,
            SubscriptionHelper SubHelper,
            SignInManager<AuctionUser> SignInManager)
        {
            this.repo = repo;
            this.roleManager = RoleManager;
            this.userManager = UserManager;
            this.SubHelper = SubHelper;
            this.SignInManager = SignInManager;
        }

        public async Task<IActionResult> Index(uint cbmin = 0, uint cbmax = 0,
            uint mbmin = 0, uint mbmax = 0,
            string states = "", string propertyType = "", string sellerType = "",
            string sortby = "bm", string displaykey = "Comfortable", bool favonly = false)
        {
            #region PDT bug workaround
            var user = await this.userManager.GetUserAsync(User);
            //if (!user.SubscriptionConfirmed &&
            //        user.SubscribedAt.AddHours(6) < DateTime.UtcNow)
            //{
            //    await SubHelper.CancelUserSubscription(user.Email);
            //    await SignInManager.RefreshSignInAsync(user);
            //    return RedirectToAction("manage", "Account");
            //}
            #endregion


            var statesList = string.IsNullOrEmpty(states) ? new List<string>() : states.Split(',').ToList();
            var propertyList = string.IsNullOrEmpty(propertyType) ? new List<string>() : propertyType.Split(',').ToList();
            var sellerList = string.IsNullOrEmpty(sellerType) ? new List<string>() : sellerType.Split(',').ToList();
            var filter = repo.GenerateFullPredicate(user.Id, cbmin, cbmax, mbmin, mbmax, statesList, propertyList, sellerList, favonly);
            var filterExceptState = repo.GeneratePredicateWithoutState(user.Id, cbmin, cbmax, mbmin, mbmax, statesList, propertyList, sellerList, favonly);
            var filterExceptPropertyType = repo.GeneratePredicateWithoutPropertyType(user.Id, cbmin, cbmax, mbmin, mbmax, statesList, propertyList, sellerList, favonly);
            var filterExceptSellerType = repo.GeneratePredicateWithoutSellerType(user.Id, cbmin, cbmax, mbmin, mbmax, statesList, propertyList, sellerList, favonly);
            var filteredAuctions = repo.GetAllAuctions(out var count, filter, sortby);
            var filteredAuctionsWithoutStateRestriction = repo.GetAllAuctions(out _, filterExceptState, sortby);
            var filteredAuctionsWithoutTypeRestriction = repo.GetAllAuctions(out _, filterExceptPropertyType, sortby);
            var filteredAuctionsWithoutSellerRestriction = repo.GetAllAuctions(out _, filterExceptSellerType, sortby);
            ViewBag.host = Request.Host.Value;
            ViewBag.minCurrentbid = repo.GetAuctionsCurrentBidMin();
            ViewBag.maxCurrentbid = repo.GetAuctionsCurrentBidMax();
            ViewBag.cbmin = cbmin == 0 ? ViewBag.minCurrentbid : cbmin;
            ViewBag.cbmax = cbmax == 0 ? ViewBag.maxCurrentbid : cbmax;
            ViewBag.minMbid = repo.GetAuctionsMBidMin();
            ViewBag.maxMbid = repo.GetAuctionsMBidMax();
            ViewBag.mbmin = mbmin == 0 ? ViewBag.minMbid : mbmin;
            ViewBag.mbmax = mbmax == 0 ? ViewBag.maxMbid : mbmax;
            var auctionStates = repo.GetAuctionsStates(statesList, filteredAuctionsWithoutStateRestriction);
            ViewBag.states = auctionStates.OrderBy(s => s.StateName);
            ViewBag.propertyType = repo.GetPropertyList(propertyList, filteredAuctionsWithoutTypeRestriction);
            ViewBag.sellerType = repo.GetSellerList(sellerList, filteredAuctionsWithoutSellerRestriction);
            //ViewBag.resultsFound = count;
            ViewBag.resultsFound = filteredAuctions.Where(a => a.AuctionCloses >= DateTime.Now).Count(); // by me
            ViewBag.sort = sortby;
            ViewBag.denisty = displaykey;
            ViewBag.favAuctions = repo.GetFavAuctions(user.Id);
            ViewBag.favonly = favonly;
            var auctions = filteredAuctions.Where(a => a.AuctionCloses >= DateTime.Now).Skip(0).Take(20).ToList();
            //List<Auction> auctions = new List<Auction>(auctList.Where(a => a.AuctionCloses >= DateTime.Now));
            auctions.ForEach(a => a.Link = "https://www.bid4assets.com" + a.Link);
            
            return View(auctions);
        }

        public async Task<IActionResult> IndexAjax(ushort skip, uint cbmin = 0, uint cbmax = 0,
            uint mbmin = 0, uint mbmax = 0, string states = "", string propertyType = "", string sellerType = "",
            string sortby = "bm", string displaykey = "Comfortable", bool favonly = false)
        {
            var statesList = string.IsNullOrEmpty(states) ? new List<string>() : states.Split(',').ToList();
            var propertyList = string.IsNullOrEmpty(propertyType) ? new List<string>() : propertyType.Split(',').ToList();
            var sellerList = string.IsNullOrEmpty(sellerType) ? new List<string>() : sellerType.Split(',').ToList();
            ViewBag.host = Request.Host.Value;
            var user = await this.userManager.GetUserAsync(User);
            ViewBag.favAuctions = repo.GetFavAuctions(user.Id);
            ViewBag.favonly = favonly;
            var filter = repo.GenerateFullPredicate(user.Id, cbmin, cbmax, mbmin, mbmax, statesList, propertyList, sellerList, favonly);
            var results = repo.GetAuctions(out var count, filter, sortby, skip);

            results.ForEach(a => a.Link = "https://www.bid4assets.com" + a.Link);

            if (displaykey == "Comfortable")
                return results.Any()
                    ? PartialView("_AuctionsPartial", results.Where(a => a.AuctionCloses >= DateTime.Now).Distinct().ToList())
                    : (IActionResult)BadRequest("non found");
            return results.Any()
                ? PartialView("_CompactAuctionsPartial", results.Where(a => a.AuctionCloses >= DateTime.Now).Distinct().ToList())
                : (IActionResult)BadRequest("non found");
        }

        public async Task<IActionResult> FavAuctionToggle(Guid AuctionId)
        {
            var user = await this.userManager.GetUserAsync(User);
            if (repo.GetFavAuctions(user.Id).All(a => a != AuctionId))
            {
                repo.AddFavAuction(new UserFavAuction
                {
                    AuctionId = AuctionId,
                    AuctionUserId = user.Id
                });
                return Ok();
            }
            else
            {
                repo.RemoveFavAuction(user.Id, AuctionId);
                return Ok("removed");
            }
        }

        [Produces("application/json")]
        [HttpGet]
        public async Task<IActionResult> SearchState(string state)
        {
            try
            {
                var statesList = new List<string>();

                if (!String.IsNullOrEmpty(state))
                {
                    state = state.ToLower();
                    statesList = (from a in dbcontext.Auctions where a.State.ToLower().StartsWith(state) select a.State).Distinct().ToList();
                }
                else
                {
                    statesList = (from a in dbcontext.Auctions select a.State).Distinct().ToList();
                }
                return Ok(statesList);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }   

        public List<AuctionStateViewModel> GetFilterStates(string state)
        {
            var statesList = new List<string>();
            if (!String.IsNullOrEmpty(state))
            {
                state = state.ToLower();
                statesList = (from a in dbcontext.Auctions where a.State.ToLower().StartsWith(state) select a.State).Distinct().ToList();
            }
            else
            {
                statesList = (from a in dbcontext.Auctions select a.State).Distinct().ToList();
            }


            //var locationFilters = repo.GetAuctionsStates(statesList, filteredAuctionsWithoutStateRestriction);

            List<AuctionStateViewModel> locationFilters = new List<AuctionStateViewModel>();
            statesList.ForEach(s =>
            {
                locationFilters.Add(new AuctionStateViewModel()
                {
                    StateName = s,
                    Count = dbcontext.Auctions.Where(a => a.AuctionCloses >= DateTime.Now).Count(a => a.State == s),
                    //Filtered = dbcontext.Auctions.Any(a => a.State == s)
                    Filtered = false
                });
            });

            if (locationFilters != null)
            {
                ViewBag.stateFilter = locationFilters;
               return locationFilters;
            }
            else
            {
                return null;
            }
        }
    }
}