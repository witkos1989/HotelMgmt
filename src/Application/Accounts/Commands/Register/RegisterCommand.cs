using MediatR;
using FluentValidation.Results;
using Application.Accounts.Commands;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.Accounts.Commands
{
    public record RegisterCommand : IRequest<Result>
    {
        public string FirstName { get; init; }

        public string LastName { get; init; }

        public string UserName { get; init; }

        public string Email { get; init; }

        public string Password { get; init; }

        public string? ConfirmPassword { get; init; }

        public string? PhoneNumber { get; init; }

        public string? Url { get; set; }
    }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly ICustomMailSender _customMailSender;

    public RegisterCommandHandler(
        IIdentityService identityService,
        ICustomMailSender customMailSender)
    {
        _identityService = identityService;
        _customMailSender = customMailSender;
    }

    public async Task<Result> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var existingEmail = await _identityService.FindByEmailAsync(request.Email, cancellationToken);

        if (existingEmail)
        {
            try
            {
                await _customMailSender.SendAlreadyExistEmail(request.Email, cancellationToken);
            }
            catch
            {
                var message = "Nie można wysłać maila potwierdzającego. " +
                    "Proszę skontaktować się z administratorem serwisu";

                return Result.Failure(message);

            }

            return Result.Success();
        }

        var result = await _identityService.CreateUserAsync(
            request.UserName,
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.Password,
            cancellationToken);

        if (!result.result.Succeeded)
        {
            var message = "Nie można utworzyć użytkownika. Skontaktuj się z administratorem serwisu";

            return Result.Failure(message);
        }

        if (string.IsNullOrEmpty(result.token))
        {
            var message = "Nie można wygenerować klucza rejestrującego. " +
                    "Proszę skontaktować się z administratorem serwisu";

            return Result.Failure(message);
        }

        var confirmationLink = request.Url + "?token=" + Uri.EscapeDataString(result.token) +
            "&email=" + request.Email;

        try
        {
            await _customMailSender.SendActivationLinkEmail(confirmationLink, request.Email, cancellationToken);
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