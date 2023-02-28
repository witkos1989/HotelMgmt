using Application.Common.Models;

namespace Application.Common.Interfaces
{
	public interface ISigningManager
	{
		public Task<(Result, bool)> SignInAsync(string userName, string password, CancellationToken cancellationToken);

		public Task SignOutAsync(CancellationToken cancellationToken);

		public Task<Result> TwoFactorSignInAsync(string twoFactorCode, CancellationToken cancellationToken);
    }
}