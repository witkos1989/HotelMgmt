using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Accounts.Commands
{
	public record TwoStepLoginCommand : IRequest<Result>
	{
		public string TwoFactorCode { get; init; }
	}


    public class TwoStepLoginCommandHandler : IRequestHandler<TwoStepLoginCommand, Result>
    {
        private readonly ISigningManager _signingManager;

        public TwoStepLoginCommandHandler(ISigningManager signingManager)
        {
            _signingManager = signingManager;
        }

        public async Task<Result> Handle(TwoStepLoginCommand command, CancellationToken cancellationToken)
        {
            return await _signingManager.TwoFactorSignInAsync(command.TwoFactorCode, cancellationToken);
        }
    }
}