namespace Domain.Events
{
    public class ReservedRoomCompletedEvent : BaseEvent
    {
        public ReservedRoomCompletedEvent(ReservedRoom reservedRoom)
        {
            ReservedRoom = reservedRoom;
        }

        public ReservedRoom ReservedRoom { get; }
    }

    public class ReservedRoomCreatedEvent : BaseEvent
    {
        public ReservedRoomCreatedEvent(ReservedRoom reservedRoom)
        {
            ReservedRoom = reservedRoom;
        }

        public ReservedRoom ReservedRoom { get; }
    }

    public class ReservedRoomDeletedEvent : BaseEvent
    {
        public ReservedRoomDeletedEvent(ReservedRoom reservedRoom)
        {
            ReservedRoom = reservedRoom;
        }

        public ReservedRoom ReservedRoom { get; }
    }
}