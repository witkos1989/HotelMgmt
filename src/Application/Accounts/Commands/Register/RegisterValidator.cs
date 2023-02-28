using System.Text.RegularExpressions;
using FluentValidation;

namespace Application.Accounts.Commands
{
	public class RegisterValidator : AbstractValidator<RegisterCommand>
	{
		public RegisterValidator()
		{
			RuleFor(v => v.UserName)
				.NotEmpty()
				.WithMessage("Wpisz nazwę użytkownika");

			RuleFor(v => v.FirstName)
				.NotEmpty()
				.WithMessage("Wpisz imię");

			RuleFor(v => v.LastName)
				.NotEmpty()
				.WithMessage("Wpisz nazwisko");

			RuleFor(v => v.Email)
				.NotEmpty()
				.WithMessage("Wpisz e-mail")
				.EmailAddress()
				.WithMessage("Niepoprawny format");

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

			RuleFor(v => v.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Powtórz hasło")
                .Equal(v => v.Password)
				.WithMessage("Wpisane hasła nie są identyczne");

			RuleFor(v => v.PhoneNumber)
				.Matches(new Regex(@"^(?:\+\d+|\(\+?\d+\))?(?:\d{9,10}| ?\d{3}([ -])\d{3}\1\d{3,4})$",
				0,
				TimeSpan.FromMilliseconds(100)))
				.WithMessage("Niepoprawny format");
        }
	}
}