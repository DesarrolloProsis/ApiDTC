using ApiDTC.Models.Email;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.IO;

namespace ApiDTC.Services
{
    public class EmailService
    {
        private readonly ApiLogger _apiLogger;

        public EmailService(ApiLogger apiLogger)
        {
            _apiLogger = apiLogger;
        }

        public bool Send(Email email, IFormFile file)
        {
            try
            {
                var builder = new BodyBuilder();

                builder.HtmlBody = email.body;

                byte[] fileBytes;

                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }

                builder.Attachments.Add(file.FileName, fileBytes);

                var message = new MimeMessage();
                //From address
                message.From.Add(new MailboxAddress("PROSIS", "send1@grupo-prosis.com"));
                //To address
                message.To.Add(new MailboxAddress(email.addressee));
                //Subject
                message.Subject = email.affair;
                //Body
                message.Body = builder.ToMessageBody();


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
