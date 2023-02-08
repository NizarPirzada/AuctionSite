using System;
using System.Collections.Generic;
using System.Linq;
using AuctionSite.DAL.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using SuiteShared.DAL.Enums;
using SuiteShared.DAL.Models;

namespace AuctionSite.DAL
{
    public class repo
    {
        private readonly AuctionContext dbcontext;

        public repo(AuctionContext dbcontext) => this.dbcontext = dbcontext;

        public Auction GetAuctionInformation(Guid id) => dbcontext.Auctions.Include(a => a.PropertyDocuments).FirstOrDefault(a => a.AuctionId == id);

        public List<Guid> GetImagesPath(Guid id) =>
            GetAuctionInformation(id)?.PropertyDocuments?.Where(a => a.DocumentType == DocumentType.Image)?.Select(p => p.Id)?.ToList() ?? new List<Guid>();

        public string GetImagePath(Guid imageId) =>
            dbcontext.PropertyDocument.FirstOrDefault(p => p.Id == imageId)?.FilePath;

        //public 


        public ExpressionStarter<Auction> GenerateFullPredicate(string userId, 
            double cbmin = 0, double cbmax = 0, double mbmin = 0, double mbmax = 0,
            List<string> states = null, List<string> propertyList = null, List<string> sellerList = null, bool favonly = false)
        {
            var predicate = PredicateBuilder.New<Auction>(true);
            if (cbmin != 0 || cbmax != 0) predicate.And(a => a.CurrentBid >= cbmin && a.CurrentBid <= cbmax);
            if (mbmin != 0 || mbmax != 0) predicate.And(a => a.MinimumBid >= mbmin && a.MinimumBid <= mbmax);
            if (states != null && states.Any()) predicate.And(a => states.Any(s => s == a.State));
            if (propertyList != null && propertyList.Any())
                predicate.And(a =>
                (propertyList.Contains("home") && a.RealEstateType != "land" && (a.NumberOfBathrooms != 0 || a.NumberOfBedrooms != 0)) ||
                (propertyList.Contains("land") && a.RealEstateType == "land") ||
                (propertyList.Contains("other") && a.RealEstateType != "land" && a.NumberOfBathrooms == 0 && a.NumberOfBedrooms == 0));
            if (sellerList != null && sellerList.Any())
                predicate.And(a =>
                (sellerList.Contains("Government") && a.SellerType == "Government" ||
                (sellerList.Contains("Bank") && a.SellerType == "Bank") ||
                (sellerList.Contains("Private Auction") && a.SellerType == "Private Auction")));
            if (favonly) predicate.And(a => GetFavAuctions(userId).Contains(a.AuctionId));

            return predicate;
        }

        public ExpressionStarter<Auction> GeneratePredicateWithoutState(string userId,
            double cbmin = 0, double cbmax = 0, double mbmin = 0, double mbmax = 0,
            List<string> states = null, List<string> propertyList = null, List<string> sellerList = null, bool favonly = false)
        {
            var predicate = PredicateBuilder.New<Auction>(true);
            if (cbmin != 0 || cbmax != 0) predicate.And(a => a.CurrentBid >= cbmin && a.CurrentBid <= cbmax);
            if (mbmin != 0 || mbmax != 0) predicate.And(a => a.MinimumBid >= mbmin && a.MinimumBid <= mbmax);
            if (propertyList != null && propertyList.Any())
                predicate.And(a =>
                (propertyList.Contains("home") && a.RealEstateType != "land" && (a.NumberOfBathrooms != 0 || a.NumberOfBedrooms != 0)) ||
                (propertyList.Contains("land") && a.RealEstateType == "land") ||
                (propertyList.Contains("other") && a.RealEstateType != "land" && a.NumberOfBathrooms == 0 && a.NumberOfBedrooms == 0));
            if (sellerList != null && sellerList.Any())
                predicate.And(a =>
                (sellerList.Contains("Government") && a.SellerType == "Government" ||
                (sellerList.Contains("Bank") && a.SellerType == "Bank") ||
                (sellerList.Contains("Private Auction") && a.SellerType == "Private Auction")));
            if (favonly) predicate.And(a => GetFavAuctions(userId).Contains(a.AuctionId));
            return predicate;
        }

        public ExpressionStarter<Auction> GeneratePredicateWithoutPropertyType(string userId,
            double cbmin = 0, double cbmax = 0, double mbmin = 0, double mbmax = 0,
            List<string> states = null, List<string> propertyList = null, List<string> sellerList = null, bool favonly = false)
        {
            var predicate = PredicateBuilder.New<Auction>(true);
            if (cbmin != 0 || cbmax != 0) predicate.And(a => a.CurrentBid >= cbmin && a.CurrentBid <= cbmax);
            if (mbmin != 0 || mbmax != 0) predicate.And(a => a.MinimumBid >= mbmin && a.MinimumBid <= mbmax);
            if (states != null && states.Any()) predicate.And(a => states.Any(s => s == a.State));
            //if (sellerList.Count> 0 && sellerList.Any()) predicate.And(a => sellerList.Any(s => s == a.SellerType)); // by me
            if (sellerList != null && sellerList.Any())
                predicate.And(a =>
                (sellerList.Contains("Government") && a.SellerType == "Government" ||
                (sellerList.Contains("Bank") && a.SellerType == "Bank") ||
                (sellerList.Contains("Private Auction") && a.SellerType == "Private Auction")));
            if (favonly) predicate.And(a => GetFavAuctions(userId).Contains(a.AuctionId));
            return predicate;
        }

        public ExpressionStarter<Auction> GeneratePredicateWithoutSellerType(string userId,
         double cbmin = 0, double cbmax = 0, double mbmin = 0, double mbmax = 0,
         List<string> states = null, List<string> propertyList = null, List<string> sellerList = null, bool favonly = false)
        {
            var predicate = PredicateBuilder.New<Auction>(true);
            if (cbmin != 0 || cbmax != 0) predicate.And(a => a.CurrentBid >= cbmin && a.CurrentBid <= cbmax);
            if (mbmin != 0 || mbmax != 0) predicate.And(a => a.MinimumBid >= mbmin && a.MinimumBid <= mbmax);
            if (states != null && states.Any()) predicate.And(a => states.Any(s => s == a.State));
            //if (propertyList.Count > 0 && propertyList.Any()) predicate.And(a => propertyList.Any(s => s == a.RealEstateType)); // by me
            if (propertyList != null && propertyList.Any())
                predicate.And(a =>
                (propertyList.Contains("home") && a.RealEstateType != "land" && (a.NumberOfBathrooms != 0 || a.NumberOfBedrooms != 0)) ||
                (propertyList.Contains("land") && a.RealEstateType == "land") ||
                (propertyList.Contains("other") && a.RealEstateType != "land" && a.NumberOfBathrooms == 0 && a.NumberOfBedrooms == 0));
            if (favonly) predicate.And(a => GetFavAuctions(userId).Contains(a.AuctionId));
            return predicate;
        }


        public List<Auction> GetAuctions(out int resultsFound,
            ExpressionStarter<Auction> predicate, string sort, ushort skip = 0)
        {
            //resultsFound = dbcontext.Auctions.Count(predicate);
            resultsFound = dbcontext.Auctions.Where(a => a.AuctionCloses >= DateTime.Now).Count(predicate);  // by me

            switch (sort)
            {
                case "mbhl":
                    return dbcontext.Auctions.Where(predicate).OrderByDescending(a => a.MinimumBid).Skip(skip).Take(20).Include(a => a.PropertyDocuments).ToList();
                case "mblh":
                    return dbcontext.Auctions.Where(predicate).OrderBy(a => a.MinimumBid).Skip(skip).Take(20).Include(a => a.PropertyDocuments).ToList();
                case "cbhl":
                    return dbcontext.Auctions.Where(predicate).OrderByDescending(a => a.CurrentBid).Skip(skip).Take(20).Include(a => a.PropertyDocuments).ToList();
                case "cblh":
                    return dbcontext.Auctions.Where(predicate).OrderBy(a => a.CurrentBid).Skip(skip).Take(20).Include(a => a.PropertyDocuments).ToList();
                case "cthl":
                    return dbcontext.Auctions.Where(predicate).OrderByDescending(a => a.AuctionCloses).Skip(skip).Take(20).Include(a => a.PropertyDocuments).ToList();
                case "ctlh":
                    return dbcontext.Auctions.Where(predicate).OrderBy(a => a.AuctionCloses).Skip(skip).Take(20).Include(a => a.PropertyDocuments).ToList();
                case "bm":
                default:
                    return dbcontext.Auctions.Where(predicate).Skip(skip).Take(20).Include(a => a.PropertyDocuments).ToList();
            }
        }

        public List<Guid> GetFavAuctions(string id) => dbcontext.UserFavAuctions
            .Where(a => a.AuctionUserId == id).Select(a => a.AuctionId).ToList();

        public List<Auction> GetAllAuctions(out int resultsFound,
            ExpressionStarter<Auction> predicate, string sort)
        {
            resultsFound = dbcontext.Auctions.Where(a => a.AuctionCloses >= DateTime.Now).Count(predicate);
            switch (sort)
            {
                case "mbhl":
                    return dbcontext.Auctions.Where(predicate).Include(a => a.PropertyDocuments).OrderByDescending(a => a.MinimumBid).ToList();
                case "mblh":
                    return dbcontext.Auctions.Where(predicate).Include(a => a.PropertyDocuments).OrderBy(a => a.MinimumBid).ToList();
                case "cbhl":
                    return dbcontext.Auctions.Where(predicate).Include(a => a.PropertyDocuments).OrderByDescending(a => a.CurrentBid).ToList();
                case "cblh":
                    return dbcontext.Auctions.Where(predicate).Include(a => a.PropertyDocuments).OrderBy(a => a.CurrentBid).ToList();
                case "cthl":
                    return dbcontext.Auctions.Where(predicate).Include(a => a.PropertyDocuments).OrderByDescending(a => a.AuctionCloses).ToList();
                case "ctlh":
                    return dbcontext.Auctions.Where(predicate).Include(a => a.PropertyDocuments).OrderBy(a => a.AuctionCloses).ToList();
                case "bm":
                default:
                    return dbcontext.Auctions.Where(predicate).Include(a => a.PropertyDocuments).ToList();
            }

        }

        public List<AuctionStateViewModel> GetAuctionsStates(List<string> states,
            List<Auction> filteredAuctions) => filteredAuctions.Select(a => a.State).Distinct().Select(s => new AuctionStateViewModel()
            {
                StateName = s,
                Count = filteredAuctions.Where(a => a.AuctionCloses >= DateTime.Now).Count(a => a.State == s),
                Filtered = states.Any(st => st == s)
            }).ToList();

        public List<PropertyTypeViewModel> GetPropertyList(List<string> properties,
            List<Auction> filteredAuctions) => new List<PropertyTypeViewModel>()
            {
                new PropertyTypeViewModel()
                {
                    Name = "home",
                    Count = filteredAuctions.Where(a => a.AuctionCloses >= DateTime.Now).Count(a => a.RealEstateType != "land" && (a.NumberOfBathrooms != 0 || a.NumberOfBedrooms != 0)),
                    Filtered = properties.Contains("home")
                },
                new PropertyTypeViewModel()
                {
                    Name = "land",
                    Count = filteredAuctions.Where(a => a.AuctionCloses >= DateTime.Now).Count(a => a.RealEstateType == "land"),
                    Filtered = properties.Contains("land")
                },
                new PropertyTypeViewModel()
                {
                    Name = "other",
                    Count = filteredAuctions.Where(a => a.AuctionCloses >= DateTime.Now).Count(a => a.RealEstateType != "land" && (a.NumberOfBathrooms == 0 && a.NumberOfBedrooms == 0)),
                    Filtered = properties.Contains("other")
                }
            };

        public List<SellerTypeViewModel> GetSellerList(List<string> sellers,
            List<Auction> filteredAuctions) => new List<SellerTypeViewModel>()
            {
                new SellerTypeViewModel()
                {
                    Name = "Government",
                    Count = filteredAuctions.Where(a => a.AuctionCloses >= DateTime.Now).Count(a => a.SellerType == "Government"),
                    Filtered = sellers.Contains("Government")
                },
                new SellerTypeViewModel()
                {
                    Name = "Bank",
                    Count = filteredAuctions.Where(a => a.AuctionCloses >= DateTime.Now).Count(a => a.SellerType == "Bank"),
                    Filtered = sellers.Contains("Bank")
                },
                new SellerTypeViewModel()
                {
                    Name = "Private Auction",
                    Count = filteredAuctions.Where(a => a.AuctionCloses >= DateTime.Now).Count(a => a.SellerType == "Private Auction"),
                    Filtered = sellers.Contains("Private Auction")
                }
            };


        //public double GetAuctionsCurrentBidMin(List<Auction> filteredAuctions) => filteredAuctions.Min(a => a.CurrentBid);
        //public double GetAuctionsCurrentBidMax(List<Auction> filteredAuctions) => filteredAuctions.Max(a => a.CurrentBid);
        public double GetAuctionsCurrentBidMin() => dbcontext.Auctions.Min(a => a.CurrentBid);
        public double GetAuctionsCurrentBidMax() => dbcontext.Auctions.Max(a => a.CurrentBid);
        //public double GetAuctionsMBidMin(List<Auction> filteredAuctions) => filteredAuctions.Min(a => a.MinimumBid);
        //public double GetAuctionsMBidMax(List<Auction> filteredAuctions) => filteredAuctions.Max(a => a.MinimumBid);
        public double GetAuctionsMBidMin() => dbcontext.Auctions.Min(a => a.MinimumBid);
        public double GetAuctionsMBidMax() => dbcontext.Auctions.Max(a => a.MinimumBid);

        public void AddFavAuction(UserFavAuction userFavAuction)
        {
            dbcontext.UserFavAuctions.Add(userFavAuction);
            dbcontext.SaveChanges();
        }

        public void RemoveFavAuction(string userId, Guid auctionId)
        {
            dbcontext.UserFavAuctions.Remove(
                dbcontext.UserFavAuctions.First(a => a.AuctionUserId == userId && a.AuctionId == auctionId));
            dbcontext.SaveChanges();
        }
    }
}
