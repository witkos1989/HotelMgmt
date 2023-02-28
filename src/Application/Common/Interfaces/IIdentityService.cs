using Application.Common.DTOs;
using System.Security.Claims;
using Application.Common.Models;

namespace Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<AccountDetailsDTO?> GetUserByIdAsync(string? userId, CancellationToken cancellationToken);

        Task<bool> CheckForExistingUserNameAsync(string userId, string userName, CancellationToken cancellationToken);

        Task<bool> CheckForExistingUserMailAsync(string userId, string userMail, CancellationToken cancellationToken);

        Task<string?> GetUserNameAsync(string userId, CancellationToken cancellationToken);

        Task<string?> GetUserMailAsync(string userName, CancellationToken cancellationToken);

        Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellationToken);

        Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellationToken);

        Task<bool> FindByEmailAsync(string email, CancellationToken cancellationToken);

        Task<(Result result, string? token)> CreateUserAsync(string userName,
                                                       string firstName,
                                                       string lastName,
                                                       string email,
                                                       string? phoneNumber,
                                                       string password,
                                                       CancellationToken cancellationToken);

        Task<(Result, bool)> UpdateUserAsync(AccountDetailsDTO oldUser, AccountDetailsDTO updatedUser, CancellationToken cancellationToken);

        Task<Result> DeleteUserAsync(string userId, CancellationToken cancellationToken);

        Task<string?> GenerateTwoFactorTokenAsync(string userName, CancellationToken cancellationToken);

        Task<Result> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken);

        Task<IList<string>> GetValidTwoFactorProviderAsync(string userName, CancellationToken cancellationToken);

        Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken);

        Task<Result> ChangePasswordAsync(string userId, string oldPassword, string newPassword, CancellationToken cancellationToken);

        Task<Result> ResetPasswordAsync(string email, string token, string password, CancellationToken cancellationToken);
    }
}