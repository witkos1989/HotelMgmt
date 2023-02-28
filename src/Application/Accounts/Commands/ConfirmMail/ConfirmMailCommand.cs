using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Accounts.Commands
{
	public record ConfirmMailCommand : IRequest<Result>
	{
        public string Token { get; }

        public string Email { get; }

        public ConfirmMailCommand(string email, string token)
        {
            Email = email;

            Token = token;
        }
    }

    public class ConfirmMailCommandHandler : IRequestHandler<ConfirmMailCommand, Result>
    {
        private readonly IIdentityService _identityService;

        public ConfirmMailCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result> Handle(
            ConfirmMailCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _identityService.ConfirmEmailAsync(request.Email, request.Token, cancellationToken);

            return result;
        }
    }
}