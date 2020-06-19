namespace FunctionalLiving.Domain
{
    using Knx;
    using ValueObjects;

    public class HomeAutomationObject<T> where T : HomeAutomationObjectId
    {
        public T Id { get; }

        public HomeAutomationObjectType ObjectType { get; }

        public string Description { get; }

        public HomeAutomationObject(
            T id,
            HomeAutomationObjectType objectType,
            string description)
        {
            Id = id;
            ObjectType = objectType;
            Description = description;
        }

        protected HomeAutomationObject(
            HomeAutomationObjectType objectType,
            KnxObject knxObject,
            KnxObject? knxFeedbackObject)
        {
            Id = HomeAutomationObjectId.CreateDeterministicId<T>(knxObject.Address);
            ObjectType = objectType;
            Description = knxObject.Description;
        }
    }
}
