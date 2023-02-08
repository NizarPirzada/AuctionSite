using Microsoft.Extensions.Configuration;

namespace AuctionSite.Helpers
{
    public class PaypalHelper
    {
        private IConfiguration config { get; set; }
        public string clientId, clientSecret, mode, connectionTimeout, requestRetries, PdtToken, PaypalUrl;

        public PaypalHelper(IConfiguration config)
        {
            this.config = config;
            this.clientId = config.GetSection("PayPal")["ClientId"];
            this.clientSecret = config.GetSection("PayPal")["clientSecret"];
            this.mode = config.GetSection("PayPal")["mode"];
            this.connectionTimeout = config.GetSection("PayPal")["connectionTimeout"];
            this.requestRetries = config.GetSection("PayPal")["requestRetries"];
            this.PdtToken = config.GetSection("PayPal")["PdtToken"];
            this.PaypalUrl = config.GetSection("PayPal")["PaypalUrl"];
        }
    }
}
