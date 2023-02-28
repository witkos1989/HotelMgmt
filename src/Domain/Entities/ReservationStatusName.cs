namespace Domain.Entities
{
	public class ReservationStatusName : BaseEntity
    {
		public string Name { get; set; }

		public ICollection<ReservationStatus> ReservationStatuses { get; set; }
	}
}