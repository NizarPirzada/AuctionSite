using System;
using System.Linq;
using System.Threading.Tasks;
using AuctionSite.DAL;
using AuctionSite.DAL.Identity;
using AuctionSite.DAL.Models;
using AuctionSite.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuctionSite.Controllers
{
    [RequireHttps]
    public class AuthController : Controller
    {
        private readonly SignInManager<AuctionUser> signInManager;
        private readonly UserManager<AuctionUser> userManager;
        private readonly SMTPHelper emailSender;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly AuctionContext context;

        public AuthController(SignInManager<AuctionUser> signInManager,
            UserManager<AuctionUser> UserManager, SMTPHelper emailSender,
            RoleManager<IdentityRole> RoleManager,
            AuctionContext context)
        {
            this.signInManager = signInManager;
            this.userManager = UserManager;
            this.emailSender = emailSender;
            this.roleManager = RoleManager;
            this.context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string ReturnUrl = null)
        {
            if (!this.ModelState.IsValid)
                return BadRequest($"<ul>{string.Concat(this.ModelState.Values.Where(ms => ms.Errors.Any()).Select(ms => $"<li>{ms.Errors[0].ErrorMessage}</li>"))}</ul>");

            if (await this.userManager.FindByEmailAsync(model.email) != null &&
                (await this.userManager.FindByEmailAsync(model.email)).EmailConfirmed &&
                (await this.signInManager.PasswordSignInAsync(model.email, model.password, true, lockoutOnFailure: true)).Succeeded)
                if (ReturnUrl == null)
                    return Ok(Url.Action(action: "index", controller: "auctions"));
                else
                    return Url.IsLocalUrl(ReturnUrl) ? Ok(ReturnUrl) : Ok(Url.Action(action: "index", controller: "auctions"));
            else
                return BadRequest("Invalid Username/Password combination");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();
            return this.RedirectToAction("index", "home");
        }

        [HttpPost]
        public async Task<IActionResult> register(RegisterViewModel Input)
        {
            if (this.ModelState.IsValid)
            {
                var user = new AuctionUser()
                {
                    UserName = Input.email,
                    Email = Input.email
                };
                var result = (await this.userManager.CreateAsync(user, Input.password));
                if (result.Succeeded)
                    return await sendConfirmation(user.Email);
                else
                    return BadRequest($"<ul>{string.Concat(result.Errors.Select(ms => $"<li>{ms.Description}</li>"))}</ul>");
            }

            return BadRequest($"<ul>{string.Concat(this.ModelState.Values.Where(ms => ms.Errors.Any()).Select(ms => $"<li>{ms.Errors[0].ErrorMessage}</li>"))}</ul>");
        }

        [HttpGet]
        public async Task<IActionResult> sendConfirmation(string email)
        {
            var user = await this.userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest();

            if (user.ConfirmationCodeResentTimes < 5)
            {
                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                this.emailSender.SendRegistrationEmail(email,
                    Url.Action("ConfirmEmail", "auth", new { email = email, code = code }, "https", Request.Host.Value), code);

                user.ConfirmationCodeResentTimes++;
                await userManager.UpdateAsync(user);
                return Ok("sent");
            }

            return BadRequest("code resent times reached!");
        }

        [HttpGet]
        public async Task<IActionResult> sendforgetpassword(string email)
        {
            var user = await this.userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest();

            if (user.ForgetCodeResentTimes < 5)
            {
                var code = await userManager.GeneratePasswordResetTokenAsync(user);
                this.emailSender.SendForgetEmail(email,
                    Url.Action("ForgotPassword", "auth", new { email = email, code = code }, "https", Request.Host.Value), code);

                user.ForgetCodeResentTimes++;
                await userManager.UpdateAsync(user);
                return Ok("sent");
            }

            return BadRequest("forget password resent times reached!");
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword(string email, string code)
        {
            ViewBag.email = email;
            ViewBag.code = code;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> resetPassword(RegisterViewModel Input, string code)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userManager.FindByEmailAsync(Input.email);
                if (user != null)
                {
                    var result = await this.userManager.ResetPasswordAsync(user, code, Input.password);
                    return result.Succeeded
                        ? (IActionResult) Ok()
                        : BadRequest(
                            $"<ul>{string.Concat(result.Errors.Select(ms => $"<li>{ms.Description}</li>"))}</ul>");
                }
            }

            return BadRequest($"<ul>{string.Concat(this.ModelState.Values.Where(ms => ms.Errors.Any()).Select(ms => $"<li>{ms.Errors[0].ErrorMessage}</li>"))}</ul>");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string email, string code, bool ajax = false)
        {
            var user = await this.userManager.FindByEmailAsync(email);
            if (user == null || user.EmailConfirmed) return BadRequest();

            if (await userManager.ConfirmEmailAsync(user, code) == IdentityResult.Success)
            {
                await this.signInManager.SignInAsync(user, true);
                return ajax ? (IActionResult)Ok() : this.RedirectToAction("index", "Auctions");
            }
            return BadRequest();
        }

    }
}
