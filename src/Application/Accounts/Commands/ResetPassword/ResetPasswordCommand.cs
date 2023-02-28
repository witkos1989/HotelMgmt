using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Accounts.Commands
{
	public record ResetPasswordCommand : IRequest<Result>
	{
        public string Password { get; init; }

        public string ConfirmPassword { get; init; }

        public string Email { get; init; }

        public string Token { get; init; }
    }

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IIdentityService _identityService;

        public ResetPasswordCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _identityService.ResetPasswordAsync(command.Email, command.Token, command.Password, cancellationToken);

            return result;
        }
    }
}