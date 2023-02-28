namespace Domain.Entities
{
	public class Address : BaseAuditableEntity
	{
		public string City { get; set; }

		public string Street { get; set; }

		public string BuildingNumber { get; set; }

		public string? ApartmentNumber { get; set; }

		public string ZipCode { get; set; }

		public Country Country { get; set; }

		public ICollection<Guest> Guests { get; set; }

		public ICollection<Company> Companies { get; set; }
	}
}