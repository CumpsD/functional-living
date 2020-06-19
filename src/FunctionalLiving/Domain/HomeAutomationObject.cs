namespace FunctionalLiving.Domain
{
    using Knx;
    using ValueObjects;

    public enum HomeAutomationBackendType
    {
        Knx
    }

    public class HomeAutomationObject<T> where T : HomeAutomationObjectId
    {
        public T Id { get; }

        public HomeAutomationObjectType ObjectType { get; }

        internal HomeAutomationBackendType BackendType { get; }

        public string Description { get; }

        internal KnxObject? KnxObject { get; }

        public HomeAutomationObject(
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

        protected HomeAutomationObject(
            HomeAutomationObjectType objectType,
            KnxObject knxObject,
            KnxObject? knxFeedbackObject)
            : this(
                HomeAutomationObjectId.CreateDeterministicId<T>(knxObject.Address),
                objectType,
                HomeAutomationBackendType.Knx,
                knxObject.Description)
        {
            KnxObject = knxObject;
        }
    }
}
