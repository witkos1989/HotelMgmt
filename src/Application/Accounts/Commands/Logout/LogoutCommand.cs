using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Accounts.Commands
{
	public record LogoutCommand : IRequest
	{
	}

	public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
	{
		private readonly ISigningManager _signingManager;

		public LogoutCommandHandler(ISigningManager signingManager)
		{
			_signingManager = signingManager;
		}

        public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _signingManager.SignOutAsync(cancellationToken);

			return Unit.Value;
        }
    }
}