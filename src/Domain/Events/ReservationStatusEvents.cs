namespace Domain.Events
{
    public class ReservationStatusCompletedEvent : BaseEvent
    {
        public ReservationStatusCompletedEvent(ReservationStatus reservationStatus)
        {
            ReservationStatus = reservationStatus;
        }

        public ReservationStatus ReservationStatus { get; }
    }

    public class ReservationStatusCreatedEvent : BaseEvent
    {
        public ReservationStatusCreatedEvent(ReservationStatus reservationStatus)
        {
            ReservationStatus = reservationStatus;
        }

        public ReservationStatus ReservationStatus { get; }
    }

    public class ReservationStatusDeletedEvent : BaseEvent
    {
        public ReservationStatusDeletedEvent(ReservationStatus reservationStatus)
        {
            ReservationStatus = reservationStatus;
        }

        public ReservationStatus ReservationStatus { get; }
    }
}