using System.Collections.Generic;

namespace AuctionSite.DAL.HelperObjects
{
    public class email
    {
        public string subject { get; set; }
        public string body { get; set; }
        public string recipent { get; set; }
        public string sender { get; set; }
        public List<string> CC { get; set; }


        public email(string recipent, string subject, string body)
        {
            this.recipent = recipent;
            this.subject = subject;
            this.body = body;
        }

        public email(string recipent, string subject, string body, string sender, List<string> cc)
        {
            this.recipent = recipent;
            this.subject = subject;
            this.body = body;
            this.sender = sender;
            this.CC = cc;
        }
    }

}
