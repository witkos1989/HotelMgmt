namespace Domain.Events
{
	public class AddressCompletedEvent : BaseEvent
	{
		public AddressCompletedEvent(Address address)
        {
            Address = address;
        }

        public Address Address { get; }
	}

    public class AddressCreatedEvent : BaseEvent
    {
        public AddressCreatedEvent(Address address)
        {
            Address = address;
        }

        public Address Address { get; }
    }

    public class AddressDeletedEvent : BaseEvent
    {
        public AddressDeletedEvent(Address address)
        {
            Address = address;
        }

        public Address Address { get; }
    }
}