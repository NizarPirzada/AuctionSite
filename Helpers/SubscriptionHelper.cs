using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuctionSite.DAL.Identity;
using Microsoft.AspNetCore.Identity;

namespace AuctionSite.Helpers
{
    public class SubscriptionHelper
    {
        private readonly UserManager<AuctionUser> userManager;

        public SubscriptionHelper(UserManager<AuctionUser> UserManager) =>
            this.userManager = UserManager;

        public async Task ActivateUserSubscription(string email)
        {
            var user = await this.userManager.FindByEmailAsync(email);
            if (user != null &&  (await userManager.GetClaimsAsync(user)).All(c => c.Type != "paid"))
            {
                await this.userManager.AddClaimAsync(user, new Claim("paid", "paid"));
                await this.userManager.UpdateAsync(user);
            }
        }

        public async Task CancelUserSubscription(string email)
        {
            var user = await this.userManager.FindByEmailAsync(email);
            if (user != null && (await userManager.GetClaimsAsync(user)).Any(c => c.Type == "paid"))
            {
                await this.userManager.RemoveClaimAsync(user, (await userManager.GetClaimsAsync(user)).First(c => c.Type == "paid"));
                await this.userManager.UpdateAsync(user);
            }
        }
    }
}
