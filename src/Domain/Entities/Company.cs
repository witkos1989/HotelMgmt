namespace Domain.Entities
{
	public class Company : BaseAuditableEntity
	{
		public string CompanyName { get; set; }

		public string VatId { get; set; }

		public string Details { get; set; }

		public string Email { get; set; }

		public Address Address { get; set; }

		public ICollection<Guest> Guests { get; set; }
	}
}

