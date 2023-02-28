namespace Domain.Entities
{
	public class ReservationStatus : BaseAuditableEntity
	{
		public string? Details { get; set; }

		public DateTime Issued { get; set; }

		public ReservationStatusName ReservationStatusName { get; set; }

		public Reservation Reservation { get; set; }
	}
}