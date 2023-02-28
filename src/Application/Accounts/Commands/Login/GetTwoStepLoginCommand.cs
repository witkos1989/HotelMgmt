using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Application.Accounts.Commands
{
	public record GetTwoStepLoginCommand : IRequest<Result>
	{
		public string Name { get; init; }
	}

    public class GetTwoStepLoginHandler : IRequestHandler<GetTwoStepLoginCommand, Result>
    {
        private readonly ICustomMailSender _customMailSender;
        private readonly IIdentityService _identityService;

        public GetTwoStepLoginHandler(ICustomMailSender customMailSender, IIdentityService identityService)
        {
            _customMailSender = customMailSender;
            _identityService = identityService;
        }

        public async Task<Result> Handle(GetTwoStepLoginCommand command, CancellationToken cancellationToken)
        {
            var providers = await _identityService.GetValidTwoFactorProviderAsync(command.Name, cancellationToken);

            if (!providers.Contains("Email"))
            {
                throw new ArgumentException(nameof(providers));
            }

            var token = await _identityService.GenerateTwoFactorTokenAsync(command.Name, cancellationToken);

            var userMail = await _identityService.GetUserMailAsync(command.Name, cancellationToken);

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userMail))
            {
                var message = "Nie można wysłać maila potwierdzającego. " +
                        "Proszę skontaktować się z administratorem serwisu";

                return Result.Failure(message);
            }

            try
            {
                await _customMailSender.SendTwoStepVerificationCode(token, userMail, cancellationToken);
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