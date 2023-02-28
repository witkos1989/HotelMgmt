using Microsoft.AspNetCore.Http;
using MimeKit;

namespace Application.Common.Models
{
    public class Message
    {
        public List<MailboxAddress> Addresses { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public IFormFileCollection? Attachments { get; set; }

        public Message(IEnumerable<string> to, string subject, string content)
        {
            Addresses = new List<MailboxAddress>();

            Addresses.AddRange(to.Select(a => new MailboxAddress("Użytkownik", a)));
            Subject = subject;
            Content = content;
            Attachments = null;
        }

        public Message(IEnumerable<string> to, string subject, string content, IFormFileCollection attachments)
        {
            Addresses = new List<MailboxAddress>();

            Addresses.AddRange(to.Select(a => new MailboxAddress("Użytkownik", a)));
            Subject = subject;
            Content = content;
            Attachments = attachments;
        }
    }
}