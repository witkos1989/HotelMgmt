namespace Domain.Events
{
    public class GuestInvoiceCompletedEvent : BaseEvent
    {
        public GuestInvoiceCompletedEvent(GuestInvoice guestInvoice)
        {
            GuestInvoice = guestInvoice;
        }

        public GuestInvoice GuestInvoice { get; }
    }

    public class GuestInvoiceCreatedEvent : BaseEvent
    {
        public GuestInvoiceCreatedEvent(GuestInvoice guestInvoice)
        {
            GuestInvoice = guestInvoice;
        }

        public GuestInvoice GuestInvoice { get; }
    }

    public class GuestInvoiceDeletedEvent : BaseEvent
    {
        public GuestInvoiceDeletedEvent(GuestInvoice guestInvoice)
        {
            GuestInvoice = guestInvoice;
        }

        public GuestInvoice GuestInvoice { get; }
    }
}