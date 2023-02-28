using FluentValidation;

namespace Application.Accounts.Commands
{
	public class TwoStepLoginValidator : AbstractValidator<TwoStepLoginCommand>
	{
		public TwoStepLoginValidator()
		{
            RuleFor(v => v.TwoFactorCode)
                .NotEmpty()
                .WithMessage("Pole jest wymagane");
        }
	}
}