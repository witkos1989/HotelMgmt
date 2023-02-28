namespace Domain.Entities
{
	public class Guest : BaseAuditableEntity
	{
		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Email { get; set; }

		public string PhoneNumber { get; set; }

		public string Details { get; set; }

		public Address Address { get; set; }

		public bool IsCompanyEmployee { get; set; }

		public Company? Company { get; set; }

		public ICollection<GuestInvoice> GuestInvoices { get; set; }
	}
}