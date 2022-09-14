using ApiDTC.Models.Email;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;

namespace ApiDTC.Services
{
    public class EmailService
    {
        private readonly ApiLogger _apiLogger;

        public EmailService(ApiLogger apiLogger)
        {
            _apiLogger = apiLogger;
        }

        public bool Send(Email email)
        {
            try
            {
                //var builder = new BodyBuilder();

                //builder.LinkedResources.Add(email.file.Name, email.file);

                //builder.TextBody = email.body;

                var message = new MimeMessage();
                //From address
                message.From.Add(new MailboxAddress("PROSIS", "send1@grupo-prosis.com"));
                //To address
                message.To.Add(new MailboxAddress(email.addressee));
                //Subject
                message.Subject = email.affair;
                //Body
                message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = email.body
                };

                //Configuration

                using (var client = new SmtpClient())
                {
                    client.Connect("smtpout.europe.secureserver.net", 465, true);
                    client.Authenticate("send1@grupo-prosis.com", "Pr0s1s");
                    client.Send(message);
                    client.Disconnect(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog("MAIL", ex, "EmailService: Send", 3);
                return false;
            }
        }
    }
}
