namespace Domain.Entities
{
	public class Reservation : BaseAuditableEntity
	{
		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public decimal DiscountPercentage { get; set; }

		public decimal TotalPrice { get; set; } 

		public Guest Guest { get; set; }

		public ICollection<ReservedRoom> ReservedRooms { get; set; }

		public ICollection<GuestInvoice> GuestInvoices { get; set; }

		public ICollection<ReservationStatus> ReservationStatuses { get; set; }
	}
}