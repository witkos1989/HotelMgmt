namespace Domain.Entities
{
	public class Furniture : BaseEntity
    {
		public string Name { get; set; }

		public string Description { get; set; }

		public ICollection<RoomType> RoomTypes { get; set; }
	}
}