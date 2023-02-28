using MediatR;
using Application.Common.Models;
using Application.Common.Interfaces;

namespace Application.Accounts.Commands
{
	public record PasswordCommand : IRequest<Result>
	{
        public string OldPassword { get; set; }

        public string Password { get; set; }

        public string? ConfirmPassword { get; set; }

        public string? LoggedUserId { get; set; }
    }

    public class PasswordCommandHandler : IRequestHandler<PasswordCommand, Result>
    {
        private readonly IIdentityService _identityService;

        public PasswordCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result> Handle(PasswordCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(command.LoggedUserId))
            {
                var message = "Nie można zaktualizować hasła";

                return Result.Failure(message);
            }

            var result = await _identityService.ChangePasswordAsync(command.LoggedUserId, command.OldPassword, command.Password, cancellationToken);

            return result;
        }
    }
}

