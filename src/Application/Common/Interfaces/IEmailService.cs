using Application.Common.Models;

namespace Application.Common.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(Message message);

        Task SendEmailAsync(Message message, CancellationToken cancellationToken);
    }
}