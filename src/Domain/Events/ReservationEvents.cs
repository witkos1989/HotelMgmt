namespace Domain.Events
{
    public class ReservationCompletedEvent : BaseEvent
    {
        public ReservationCompletedEvent(Reservation reservation)
        {
            Reservation = reservation;
        }

        public Reservation Reservation { get; }
    }

    public class ReservationCreatedEvent : BaseEvent
    {
        public ReservationCreatedEvent(Reservation reservation)
        {
            Reservation = reservation;
        }

        public Reservation Reservation { get; }
    }

    public class ReservationDeletedEvent : BaseEvent
    {
        public ReservationDeletedEvent(Reservation reservation)
        {
            Reservation = reservation;
        }

        public Reservation Reservation { get; }
    }
}