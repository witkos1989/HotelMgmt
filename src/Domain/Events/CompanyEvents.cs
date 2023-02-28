namespace Domain.Events
{
    public class CompanyCompletedEvent : BaseEvent
    {
        public CompanyCompletedEvent(Company company)
        {
            Company = company;
        }

        public Company Company { get; }
    }

    public class CompanyCreatedEvent : BaseEvent
    {
        public CompanyCreatedEvent(Company company)
        {
            Company = company;
        }

        public Company Company { get; }
    }

    public class CompanyDeletedEvent : BaseEvent
    {
        public CompanyDeletedEvent(Company company)
        {
            Company = company;
        }

        public Company Company { get; }
    }
}