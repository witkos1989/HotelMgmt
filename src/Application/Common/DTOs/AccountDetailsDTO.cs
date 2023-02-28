namespace Application.Common.DTOs
{
	public class AccountDetailsDTO
	{
		public string Id { get; init; }

		public string UserName { get; init; }

        public string FirstName { get; init; }

		public string LastName { get; init; }

		public string Email { get; init; }

		public string? PhoneNumber { get; init; }

        public bool TwoFactorEnabled { get; init; }

        public string OldPassword { get; set; }

        public string Password { get; set; }

        public string? ConfirmPassword { get; set; }
    }
}