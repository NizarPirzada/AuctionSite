using AuctionSite.DAL.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuiteShared.DAL.Models;

namespace AuctionSite.DAL
{
    public class AuctionContext : IdentityDbContext<AuctionUser, IdentityRole, string>
    {
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<CountyTaxSales> CountyTaxSales{ get; set; }

        public DbSet<PropertyDocument> PropertyDocument { get; set; }

        public DbSet<UserFavAuction> UserFavAuctions { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //    => options.UseSqlite("Data Source=AuctionDB");

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            //options.UseSqlServer(
            //    "data source=104.156.227.115; Initial Catalog=AuctionDB; user=appuser; password=ergWER32@#$1dfWER$#%1sd;");
            options.UseSqlServer(
                "data source=DESKTOP-PR5231O\\SQLEXPRESS; Initial Catalog=AuctionDB; Trusted_Connection=True;");
        }
    }
}
