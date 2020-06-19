namespace FunctionalLiving.Domain
{
    using Knx;
    using ValueObjects;

    public class HomeAutomationObject<T> where T : HomeAutomationObjectId
    {
        public T Id { get; }

        public HomeAutomationObjectType ObjectType { get; }

        internal HomeAutomationBackendType BackendType { get; }

        public string Description { get; }

        internal KnxObject? KnxObject { get; set; }
        internal KnxObject? KnxFeedbackObject { get; set; }

        protected HomeAutomationObject(
            T id,
            HomeAutomationObjectType objectType,
            HomeAutomationBackendType backendType,
            string description)
        {
            Id = id;
            ObjectType = objectType;
            BackendType = backendType;
            Description = description;
        }
    }
}
