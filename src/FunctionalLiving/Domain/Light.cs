namespace FunctionalLiving.Domain
{
    using FunctionalLiving.Domain.Knx;
    using FunctionalLiving.ValueObjects;

    public class Light : HomeAutomationObject<LightId>
    {
        public Light(
            KnxObject knxObject,
            KnxObject? knxFeedbackObject)
            : base(HomeAutomationObjectType.Light, knxObject, knxFeedbackObject) { }
    }
}
