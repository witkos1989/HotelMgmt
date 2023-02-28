namespace Domain.Entities
{
	public class GuestInvoice : BaseAuditableEntity
	{
		public decimal Amount { get; set; }

		public TypeOfPayment TypeOfPayment { get; set; }

		public DateTime Issued { get; set; }

		public DateTime? Paid { get; set; }

		public DateTime? Cancelled { get; set; }

		public Guest Guest { get; set; }

		public Reservation Reservation { get; set; }
	}
}