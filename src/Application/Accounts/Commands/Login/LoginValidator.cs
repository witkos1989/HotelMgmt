using FluentValidation;

namespace Application.Accounts.Commands
{
	public class LoginValidator : AbstractValidator<LoginCommand>
    {
		public LoginValidator()
		{
			RuleFor(v => v.Name)
				.NotEmpty()
                .WithMessage("Wpisz nazwę użytkownika");

            RuleFor(v => v.Password)
                .NotEmpty()
                .WithMessage("Wpisz hasło");
        }
	}
}