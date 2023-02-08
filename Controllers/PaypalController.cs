using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AuctionSite.DAL.Identity;
using AuctionSite.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PayPal;

namespace AuctionSite.Controllers
{
    [RequireHttps]
    public class PaypalController : Controller
    {
        private readonly PaypalHelper ppHelper;
        private readonly SubscriptionHelper subHelper;
        private readonly UserManager<AuctionUser> UserManager;
        private readonly SignInManager<AuctionUser> SignInManager;
        private readonly SMTPHelper emailSender;

        public PaypalController(PaypalHelper ppHelper,
            SubscriptionHelper subHelper,
            UserManager<AuctionUser> UserManager,
            SignInManager<AuctionUser> SignInManager,
            SMTPHelper emailSender)
        {
            this.ppHelper = ppHelper;
            this.subHelper = subHelper;
            this.UserManager = UserManager;
            this.SignInManager = SignInManager;
            this.emailSender = emailSender;
        }

        public async Task<IActionResult> Success(string tx, string st, string amt, string cc)
        {
            var user = await this.UserManager.GetUserAsync(User);
            user.SubscribedAt = DateTime.UtcNow;
            user.SubscriptionConfirmed = false;
            await subHelper.ActivateUserSubscription(user.Email);
            await this.UserManager.UpdateAsync(user);
            await SignInManager.RefreshSignInAsync(user);
            return RedirectToAction("Confirmation");
        }

        public IActionResult Confirmation() => View("Success");

        [Authorize]
        public IActionResult Failed() => Ok("failed!");
        

        public async Task<IActionResult> webhook()
        {
            string strRequest;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.ASCII))
            {
                strRequest = reader.ReadToEnd();
            }
            string response = GetPayPalResponse(strRequest);
            if (response == "VERIFIED")
            {
                var info = strRequest.Split('&').Select(WebUtility.UrlDecode).ToDictionary(k => k.Split('=')[0], v => v.Split('=')[1]);
                switch (info["txn_type"])
                {
                    case "subscr_signup":
                        {
                            //we no longer need to verify this information
                            //as we're using a hosted button now but will just leave it there
                            //if(info["amount3"] == "19.99" && info["amount1"] == "0.00"
                            //   && info["recurring"] == "1" && info["mc_currency"] == "USD"
                            //   && info["payer_status"] == "verified")
                            await this.subHelper.ActivateUserSubscription(info["custom"]);
                            var user = await this.UserManager.FindByEmailAsync(info["custom"]);
                            user.SubscriptionConfirmed = true;
                            user.FullName = $"{info["first_name"]} {info["last_name"]}";
                            await this.UserManager.UpdateAsync(user);
                            this.emailSender.SendPremiumEmail(user.Email, user.FullName);
                            break;
                        }

                    case "subscr_failed":
                    case "subscr_cancel":
                    case "subscr_eot":
                        {
                            await this.subHelper.CancelUserSubscription(info["custom"]);
                            var user = await this.UserManager.FindByEmailAsync(info["custom"]);
                            user.SubscriptionConfirmed = false;
                            user.SubscribedAt = default(DateTime);
                            await this.UserManager.UpdateAsync(user);
                            break;
                        }
                }
                return Ok();
            }

            return BadRequest();
        }


        private string GetPayPalResponse(string strRequest)
        {
            var req = (HttpWebRequest)WebRequest.Create(ppHelper.PaypalUrl);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            strRequest = "cmd=_notify-validate&" + strRequest;
            req.ContentLength = strRequest.Length;
            string response = "";
            using (var streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII))
            {
                streamOut.Write(strRequest);
                streamOut.Close();
                using (var streamIn = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    response = streamIn.ReadToEnd();
                }
            }
            return response;
        }
    }
}
