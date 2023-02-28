using Application.Common.Models;
using Application.Common.Interfaces;
using Application.Common.DTOs;
using MediatR;
using AutoMapper;

namespace Application.Accounts.Commands
{
	public record ProfileCommand : IRequest<Result>
	{
        public string UserName { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }

        public string Email { get; init; }

        public string? PhoneNumber { get; init; }

        public bool TwoFactorEnabled { get; init; }

        public string? LoggedUserId { get; set; }
    }

    public class ProfileCommandHandler : IRequestHandler<ProfileCommand, Result>
    {
        private readonly IIdentityService _identityService;
        private readonly ISigningManager _signingManager;

        public ProfileCommandHandler(IIdentityService identityService,
            ISigningManager signingManager)
        {
            _signingManager = signingManager;
            _identityService = identityService;
        }

        public async Task<Result> Handle(ProfileCommand command, CancellationToken cancellationToken)
        {
            var user = await _identityService.GetUserByIdAsync(command.LoggedUserId, cancellationToken);

            var error = await ValidateUserDuplicationAsync(user, cancellationToken);

            if (!string.IsNullOrEmpty(error))
            {
                return Result.Failure(error);
            }

            var commandUserDTO = new AccountDetailsDTO()
            {
                Id = command.LoggedUserId!,
                UserName = command.UserName,
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                TwoFactorEnabled = command.TwoFactorEnabled,
                PhoneNumber = command.PhoneNumber
            };

            var result = await _identityService.UpdateUserAsync(user!, commandUserDTO, cancellationToken);

            if (result.Item2)
            {
                await _signingManager.SignOutAsync(cancellationToken);
            }

            return result.Item1;
        }

        private async Task<string> ValidateUserDuplicationAsync(AccountDetailsDTO? user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                var message = "Nie można odczytać danych użytkownika";

                return message;
            }

            var checkExistingUserName = await _identityService.CheckForExistingUserNameAsync(user.Id, user.UserName, cancellationToken);

            if (checkExistingUserName)
            {
                var message = "Użytkownik " + user.UserName + " już istnieje";

                return message;
            }

            var checkExistingMail = await _identityService.CheckForExistingUserMailAsync(user.Id, user.Email, cancellationToken);

            if (checkExistingMail)
            {
                var message = "W systemie jest już zarejestrowany użytkownik z takim adresem email";

                return message;
            }

            return string.Empty;
        }
    }
}