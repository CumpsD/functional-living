namespace FunctionalLiving.Domain
{
    using Knx;
    using ValueObjects;

    public class Light : HomeAutomationObject<LightId>
    {
        public Light(
            KnxObject knxObject,
            KnxObject? knxFeedbackObject)
            : base(HomeAutomationObjectType.Light, knxObject, knxFeedbackObject) { }
    }
}
