using Application.Common.Interfaces;
using Application.Common.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace Infrastructure.Services
{
    //implementing sync and async send functions, and MimeMessage builder function
    public class EmailService : IEmailService
    {
        private readonly IEmailConfiguration _emailConfiguration;

        public EmailService(IEmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);

            Send(emailMessage);
        }

        public async Task SendEmailAsync(Message message, CancellationToken cancellationToken)
        {
            var mailMessage = CreateEmailMessage(message);

            await SendAsync(mailMessage, cancellationToken);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Powiadomienie", _emailConfiguration.SmtpUsername));

            emailMessage.To.AddRange(message.Addresses);

            emailMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = string.Format("<p>{0}</p>", message.Content)
            };

            if (message.Attachments != null && message.Attachments.Any())
            {
                byte[] fileBytes;
                foreach (var attachment in message.Attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        attachment.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }

                    bodyBuilder.Attachments.Add(
                        attachment.FileName,
                        fileBytes,
                        ContentType.Parse(attachment.ContentType));
                }
            }

            emailMessage.Body = bodyBuilder.ToMessageBody();

            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();

            try
            {
                client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, true);

                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

                client.Send(mailMessage);
            }
            catch
            {
                //log an error message or throw an exception, or both.
                throw;
            }
            finally
            {
                client.Disconnect(true);

                client.Dispose();
            }
        }

        private async Task SendAsync(MimeMessage mailMessage, CancellationToken cancellationToken)
        {
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, true, cancellationToken);

                client.AuthenticationMechanisms.Remove("XOAUTH2");

                await client.AuthenticateAsync(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword, cancellationToken);

                await client.SendAsync(mailMessage, cancellationToken);
            }
            catch
            {
                //log an error message or throw an exception, or both.
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true, cancellationToken);

                client.Dispose();
            }
        }
    }
}