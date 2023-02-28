namespace Domain.Entities
{
	public class Country : BaseEntity
	{
		public string Name { get; set; }

		public ICollection<Address> Addresses { get; set; }
	}
}