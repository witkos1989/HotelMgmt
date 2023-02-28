using System.Reflection;
using System.Security.Claims;
using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Models;
using Azure.Core;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
        private readonly IAuthorizationService _authorizationService;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
            IAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _authorizationService = authorizationService;
        }

        public async Task<AccountDetailsDTO?> GetUserByIdAsync(string? userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            var user = await _userManager.Users.FirstAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                return null;
            }

            var userDTO = new AccountDetailsDTO()
            {
                Id = user.Id,
                UserName = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                TwoFactorEnabled = user.TwoFactorEnabled
            };

            return userDTO;
        }

        public async Task<bool> CheckForExistingUserNameAsync(string userId, string userName, CancellationToken cancellationToken)
        {
            var userList = await _userManager.Users
                .Where(u => u.UserName == userName && u.Id != userId)
                .ToListAsync(cancellationToken);

            if (userList.Count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> CheckForExistingUserMailAsync(string userId, string userMail, CancellationToken cancellationToken)
        {
            var userList = await _userManager.Users
                .Where(u => u.Email == userMail && u.Id != userId)
                .ToListAsync(cancellationToken);

            if (userList.Count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<string?> GetUserNameAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.FirstAsync(u => u.Id == userId, cancellationToken);

            return user.UserName;
        }

        public async Task<string?> GetUserMailAsync(string userName, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return string.Empty;
            }

            return user.Email;
        }

        public async Task<(Result result, string? token)> CreateUserAsync(string userName,
                                                                          string firstName,
                                                                          string lastName,
                                                                          string email,
                                                                          string? phoneNumber,
                                                                          string password,
                                                                          CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber
            };

            var result = await _userManager.CreateAsync(user, password);

            var token = await GenerateEmailConfiramtionToken(user.Id, cancellationToken);

            var applicationResult = result.ToApplicationResult();

            return (applicationResult, token);
        }

        public async Task<(Result, bool)> UpdateUserAsync(AccountDetailsDTO oldUser, AccountDetailsDTO updatedUser, CancellationToken cancellationToken)
        {
            var needLogout = false;

            var differences = oldUser.CompareObjects(updatedUser);

            if (differences.Count == 0)
            {
                var message = "Zapis danych nie był konieczny, ponieważ nie wykryto zmian";

                return (Result.Failure(message), needLogout);
            }

            var user = await _userManager.FindByIdAsync(oldUser.Id);

            var userType = user!.GetType();

            foreach (var difference in differences)
            {
                PropertyInfo? property = userType.GetProperty(difference.PropertyName);
                if (property is not null)
                {
                    property.SetValue(user, difference.Value2, null);
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var message = "Podczas zapisu danych wystąpił błąd";

                return (Result.Failure(message), needLogout);
            }

            if (oldUser.Email != updatedUser.Email)
            {
                await _userManager.SetEmailAsync(user, updatedUser.Email);
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _userManager.ConfirmEmailAsync(user, token);
            }

            if (oldUser.UserName != updatedUser.UserName)
            {
                await _userManager.SetUserNameAsync(user, updatedUser.UserName);

                needLogout = true;
            }

            return (Result.Success(), needLogout);
        }

        public async Task<Result> DeleteUserAsync(string userId, CancellationToken cancellationToken)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return Result.Success();
            }

            return await DeleteUserAsync(user, cancellationToken);
        }

        public async Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellationToken)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return false;
            }

            var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

            var result = await _authorizationService.AuthorizeAsync(principal, policyName);

            return result.Succeeded;
        }

        public async Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellationToken)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return false;
            }

            var result = await _userManager.IsInRoleAsync(user, role);

            return result;
        }

        public async Task<bool> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return false;
            }

            return true;
        }

        public async Task<string?> GenerateTwoFactorTokenAsync(string userName, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return string.Empty;
            }

            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            return token;
        }

        public async Task<Result> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Result.Success();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            var applicationResult = result.ToApplicationResult();

            return applicationResult;
        }

        public async Task<IList<string>> GetValidTwoFactorProviderAsync(string userName, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return new List<string>();
            }

            var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);

            return providers;
        }

        public async Task<Result> ChangePasswordAsync(string userId, string oldPassword, string newPassword, CancellationToken cancellationToken)
        {
            if (oldPassword.Equals(newPassword))
            {
                var message = "Nowe hasło nie może być takie samo jak stare";

                return Result.Failure(message);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                var message = "Nie można zaktualizować hasła";

                return Result.Failure(message);
            }

            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            var applicationResult = result.ToApplicationResult();

            return applicationResult;
        }

        public async Task<Result> ResetPasswordAsync(string email, string token, string password, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Result.Success();
            }

            var result = await _userManager.ResetPasswordAsync(user, token, password);

            var applicationResult = result.ToApplicationResult();

            return applicationResult;
        }

        public async Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return string.Empty;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return token;
        }

        private async Task<string?> GenerateEmailConfiramtionToken(string userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return string.Empty;
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            return token;
        }

        private async Task<Result> DeleteUserAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var result = await _userManager.DeleteAsync(user);

            var applicationResult = result.ToApplicationResult();

            return applicationResult;
        }
    }
}