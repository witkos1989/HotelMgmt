using FluentValidation;

namespace Application.Accounts.Commands
{
	public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
	{
		public ForgotPasswordValidator()
		{
            RuleFor(v => v.Email)
                .NotEmpty()
                .WithMessage("Wpisz e-mail")
                .EmailAddress()
                .WithMessage("Niepoprawny format");
        }
	}
}