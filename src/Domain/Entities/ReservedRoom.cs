namespace Domain.Entities
{
	public class ReservedRoom : BaseAuditableEntity
	{
		public decimal Price { get; set; }

        public Reservation Reservation { get; set; }

		public Room Room { get; set; }
	}
}