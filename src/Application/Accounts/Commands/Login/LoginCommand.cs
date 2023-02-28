using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Accounts.Commands
{
	public record LoginCommand : IRequest<(Result, bool)>
	{
        public string Name { get; init; }

        public string Password { get; init; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, (Result, bool)>
    {
        private readonly ISigningManager _signingManager;

        public LoginCommandHandler(ISigningManager signingManager)
        {
            _signingManager = signingManager;
        }

        public async Task<(Result, bool)> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = await _signingManager.SignInAsync(request.Name, request.Password, cancellationToken);

            return result;
        }
    }
}