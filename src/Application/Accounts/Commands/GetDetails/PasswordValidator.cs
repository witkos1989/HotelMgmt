using FluentValidation;

namespace Application.Accounts.Commands
{
	public class PasswordValidator : AbstractValidator<PasswordCommand>
	{
		public PasswordValidator()
		{
            RuleFor(v => v.Password)
                .NotEmpty()
                .WithMessage("Wpisz hasło")
                .MinimumLength(8)
                .WithMessage("Hasło powinno mieć co najmniej {MinLength} znaków")
                .Matches(@"[A-Z]+")
                .WithMessage("Hasło powinno zawierać co najmniej jedną dużą literę")
                .Matches(@"[a-z]+")
                .WithMessage("Hasło powinno zawierać co najmniej jedną małą literę")
                .Matches(@"[0-9]+")
                .WithMessage("Hasło powinno zawierać co najmniej jedną cyfrę")
                .Matches(@"[\!\?\*\.\@\#\$\,\%\^\&\(\)\-\=\+]+")
                .WithMessage("Hasło powinno zawierać co najmniej jeden znak specjalny");

            RuleFor(v => v.OldPassword)
                .NotEmpty()
                .WithMessage("Wpisz hasło")
                .MinimumLength(8)
                .WithMessage("Hasło powinno mieć co najmniej {MinLength} znaków")
                .Matches(@"[A-Z]+")
                .WithMessage("Hasło powinno zawierać co najmniej jedną dużą literę")
                .Matches(@"[a-z]+")
                .WithMessage("Hasło powinno zawierać co najmniej jedną małą literę")
                .Matches(@"[0-9]+")
                .WithMessage("Hasło powinno zawierać co najmniej jedną cyfrę")
                .Matches(@"[\!\?\*\.\@\#\$\,\%\^\&\(\)\-\=\+]+")
                .WithMessage("Hasło powinno zawierać co najmniej jeden znak specjalny");

            RuleFor(v => v.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Powtórz hasło")
                .Equal(v => v.Password)
                .WithMessage("Wpisane hasła nie są identyczne");
        }
	}
}