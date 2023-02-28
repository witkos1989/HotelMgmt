using Application.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public static class IdentityResultExtensions
    {
        public static Result ToApplicationResult(this IdentityResult result)
        {
            if (result.Succeeded)
                return Result.Success();
            return Result.Failure(result.Errors.Select(e => e.Description));
        }
    }
}