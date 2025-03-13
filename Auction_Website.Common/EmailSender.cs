using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

namespace Auction_Website.Common
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string apiKey = _config.GetValue<string>("MailJet:ApiKey");
            string apiSecret = _config.GetValue<string>("MailJet:ApiSecret");

            var client = new SmtpClient("in.mailjet.com", 587)
            {
                Credentials = new NetworkCredential(apiKey, apiSecret),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("bajram.shehi98@gmail.com"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }
}