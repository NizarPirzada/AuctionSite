using System;
using System.Net;
using System.Net.Mail;
using AuctionSite.DAL.HelperObjects;
using Microsoft.Extensions.Configuration;

namespace AuctionSite.Helpers
{
    public class SMTPHelper
    {
        private NetworkCredential credentials;
        public SmtpClient client;
        public IConfiguration Configuration;


        public SMTPHelper(IConfiguration configuration)
        {
            Configuration = configuration;
            var clientSettings = Configuration.GetSection("smtpclient");
            client = new SmtpClient(clientSettings["server"], ushort.Parse(clientSettings["port"]))
            {
                EnableSsl = true
            };
            credentials = new NetworkCredential(clientSettings["user"], clientSettings["pass"]);
            client.Credentials = credentials;
        }

        public SMTPHelper()
        {

        }
        
        public void SendEmail(email email)
        {
            try
            {
                using (var msg = new MailMessage(credentials.UserName, email.recipent, email.subject, email.body)
                {
                    IsBodyHtml = true
                })
                {
                    client.Send(msg);
                }
            }
            catch (Exception e)
            {
                //how would you like to handle email sending failure?
            }
        }

        public void SendQryEmail(string Sender, string Subject, string Body)
        {
            try
            {
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new NetworkCredential("tsaishenco@gmail.com", "GoAndrewGo5567.");
                SmtpServer.EnableSsl = true;


                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(Sender);
                mail.To.Add("tsaishenco@gmail.com");
                mail.Subject = Subject + " (" + Sender + ")";
                mail.Body = Body;

                SmtpServer.Send(mail);
            }

            catch (Exception e)
            {

            }

        }

        public void SendPremiumEmail(string recipient, string Name)
        {
            var EmailSection = Configuration.GetSection("emails").GetSection("invoice");
            SendEmail(new email(recipient, EmailSection["subject"],
                EmailSection["body"]
                    .Replace("[name]", Name)));
        }

        public void SendForgetEmail(string recipient, string link, string code)
        {
            var EmailSection = Configuration.GetSection("emails").GetSection("forgot");
            SendEmail(new email(recipient, EmailSection["subject"],
                EmailSection["body"]
                    .Replace("[email]", recipient.Contains('@') ? recipient.Substring(0, recipient.IndexOf('@')) : recipient)
                    .Replace("[link]", link)));
        }

        public void SendRegistrationEmail(string recipient, string link, string code)
        {
            var EmailSection = Configuration.GetSection("emails").GetSection("registration");
            SendEmail(new email(recipient, EmailSection["subject"],
                EmailSection["body"]
                    .Replace("[email]", recipient.Contains('@') ? recipient.Substring(0, recipient.IndexOf('@')) : recipient)
                    .Replace("[code]", code)
                    .Replace("[link]", link)));
        }
    }
}
