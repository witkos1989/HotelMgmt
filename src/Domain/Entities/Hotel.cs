namespace Domain.Entities
{
	public class Hotel : BaseEntity
    {
		public string Name { get; set; }

		public string Description { get; set; }

		public ICollection<Room> Rooms { get; set; }
    }
}