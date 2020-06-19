namespace FunctionalLiving.Domain
{
    using FunctionalLiving.Domain.Knx;
    using FunctionalLiving.ValueObjects;

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

        public HomeAutomationObject(
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
