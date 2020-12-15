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

        public bool Send(string to, string subject, string name)
        {
            try
            {
                var message = new MimeMessage();
                //From address
                message.From.Add(new MailboxAddress("PROSIS", "send1@grupo-prosis.com"));
                //To address
                message.To.Add(new MailboxAddress(name, to));
                //Subject
                message.Subject = subject;
                //Body
                message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = "<h1>HOLA</h1>"
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
