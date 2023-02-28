using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Accounts.Commands
{
    public record ForgotPasswordCommand : IRequest<Result>
	{
        public string Email { get; init; }

        public string? Url { get; set; }
    }


    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
    {
        private readonly IIdentityService _identityService;
        private readonly ICustomMailSender _customMailSender;

        public ForgotPasswordCommandHandler(IIdentityService identityService,
            ICustomMailSender customMailSender)
        {
            _identityService = identityService;
            _customMailSender = customMailSender;
        }

        public async Task<Result> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            var token = await _identityService.GeneratePasswordResetTokenAsync(command.Email, cancellationToken);

            if (string.IsNullOrEmpty(token))
            {
                var message = "Nie można zresetować hasła. Skontaktuje się z administratorem";

                return Result.Failure(message);
            }

            var callback = command.Url + "?token=" + Uri.EscapeDataString(token)
                + "&email=" + command.Email;

            try
            {
                await _customMailSender.SendResetPasswordMail(callback, command.Email, cancellationToken);
            }
            catch
            {
                var message = "Nie można wysłać maila potwierdzającego. " +
                        "Proszę skontaktować się z administratorem serwisu";

                return Result.Failure(message);
            }

            return Result.Success();
        }
    }
}