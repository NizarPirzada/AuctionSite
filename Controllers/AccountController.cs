using System.Threading.Tasks;
using AuctionSite.DAL.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuctionSite.Controllers
{
    [RequireHttps]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<AuctionUser> UserManager;
        public AccountController(UserManager<AuctionUser> UserManager) =>
            this.UserManager = UserManager;

        public async Task<IActionResult> Manage()

        {
            ViewBag.host = Request.Host.Value;
            return View();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> changePassword(string currentpass,
            string pass, string passConfirmation)
        {
            if (pass == passConfirmation && ModelState.IsValid)
            {
                var usr = await this.UserManager.GetUserAsync(User);
                var result = await 
                    this.UserManager.ChangePasswordAsync(usr,
                        currentpass, pass);
                await this.UserManager.UpdateAsync(usr);
                if (result.Succeeded) return Ok();
            }
            return BadRequest();
        }
    }
}