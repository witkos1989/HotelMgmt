using System.Text.RegularExpressions;
using FluentValidation;

namespace Application.Accounts.Commands
{
	public class ProfileValidator : AbstractValidator<ProfileCommand>
    {
		public ProfileValidator()
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

            RuleFor(v => v.PhoneNumber)
                .Matches(new Regex(@"^(?:\+\d+|\(\+?\d+\))?(?:\d{9,10}| ?\d{3}([ -])\d{3}\1\d{3,4})$",
                0, TimeSpan.FromMilliseconds(100)))
                .WithMessage("Niepoprawny format");
        }
	}
}