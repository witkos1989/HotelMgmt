using Application.Common.Interfaces;
using Application.Common.Models;
using Duende.IdentityServer.Models;

namespace Infrastructure.Services
{
	public class CustomMailSender : ICustomMailSender
	{
        private readonly IEmailService _emailService;

        public CustomMailSender(IEmailService emailService)
		{
            _emailService = emailService;
		}

        public async Task SendActivationLinkEmail(string confirmationLink, string email, CancellationToken cancellationToken)
        {
            var subject = "Link aktywujący konto w serwisie";

            var content = "Dziękujemy za rejestrację w naszym portalu. Poniżej znajduje się link aktywujący konto: <br \\>" + confirmationLink;

            var message = new Message(new string[] { email }, subject, content);

            await _emailService.SendEmailAsync(message, cancellationToken);
        }

        public async Task SendAlreadyExistEmail(string email, CancellationToken cancellationToken)
        {
            var subject = "Informacja dotycząca konta w systemie";

            var content = "Posiadasz już konto w naszym systemie. " +
                "Jeśli nie pamiętasz hasła do konta, kliknij w link znajdujący się na stronie logowania";

            var message = new Message(new string[] { email }, subject, content);

            await _emailService.SendEmailAsync(message, cancellationToken);
        }

        public async Task SendTwoStepVerificationCode(string token, string email, CancellationToken cancellationToken)
        {
            var subject = "Kod weryfikacyjny";

            var content = "Twój kod weryfikacyjny:<br \\><h2>" + token + "</h2>";

            var message = new Message(new string[] { email }, subject, content);

            await _emailService.SendEmailAsync(message, cancellationToken);
        }

        public async Task SendResetPasswordMail(string callback, string email, CancellationToken cancellationToken)
        {
            var subject = "Resetowanie hasła w serwisie";

            var content = "Aby zresetować hasło należy kliknąć w poniższy link: <br \\>" + callback;

            var message = new Message(new string[] { email }, subject, content);

            await _emailService.SendEmailAsync(message, cancellationToken);
        }
    }
}