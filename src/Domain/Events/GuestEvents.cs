namespace Domain.Events
{
    public class GuestCompletedEvent : BaseEvent
    {
        public GuestCompletedEvent(Guest guest)
        {
            Guest = guest;
        }

        public Guest Guest { get; }
    }

    public class GuestCreatedEvent : BaseEvent
    {
        public GuestCreatedEvent(Guest guest)
        {
            Guest = guest;
        }

        public Guest Guest { get; }
    }

    public class GuestDeletedEvent : BaseEvent
    {
        public GuestDeletedEvent(Guest guest)
        {
            Guest = guest;
        }

        public Guest Guest { get; }
    }
}