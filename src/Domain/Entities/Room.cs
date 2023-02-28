namespace Domain.Entities
{
	public class Room : BaseEntity
    {
		public string Name { get; set; }

		public string Description { get; set; }

		public decimal CurrentPrice { get; set; }

		public Hotel Hotel { get; set; }

		public RoomType RoomType { get; set; }
	}
}