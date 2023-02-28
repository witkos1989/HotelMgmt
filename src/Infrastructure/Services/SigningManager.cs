
using Application.Common.Interfaces;
using Application.Common.Models;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class SigningManager : ISigningManager
    {
        SignInManager<ApplicationUser> _signInManager;

        public SigningManager(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<(Result, bool)> SignInAsync(string userName, string password, CancellationToken cancellationToken)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(
                userName,
                password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (signInResult.RequiresTwoFactor)
            {
                return (Result.Success(), signInResult.RequiresTwoFactor);
            }

            if (!signInResult.Succeeded)
            {
                var message = "Nieprawidłowy login lub hasło";

                return (Result.Failure(message), false);
            }

            return (Result.Success(), false);
        }

        public async Task SignOutAsync(CancellationToken cancellationToken)
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<Result> TwoFactorSignInAsync(string twoFactorCode, CancellationToken cancellationToken)
        {
            string message = "";

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                message = "Nieudana próba logowania";

                return Result.Failure(message);
            }

            var result = await _signInManager.TwoFactorSignInAsync("Email", twoFactorCode, false, false);

            if (result.IsLockedOut)
            {
                message = "Konto jest zablokowane";

                return Result.Failure(message);
            }

            if (!result.Succeeded)
            {
                message = "Nieudana próba logowania";

                return Result.Failure(message);
            }

            return Result.Success();
        }
    }
}