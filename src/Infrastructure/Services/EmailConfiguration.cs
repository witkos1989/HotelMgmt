using Application.Common.Interfaces;

namespace Infrastructure.Services
{
#pragma warning disable CS8618
    public class EmailConfiguration : IEmailConfiguration
    {
        public string SmtpServer { get; set; }

        public int SmtpPort { get; set; }

        public string SmtpUsername { get; set; }

        public string SmtpPassword { get; set; }
    }
#pragma warning restore CS8618
}