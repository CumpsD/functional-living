namespace FunctionalLiving.Domain
{
    using System;
    using Knx;
    using ValueObjects;

    public class Light : HomeAutomationObject<LightId>
    {
        public LightStatus Status { get; set; } = LightStatus.Unknown;

        public Light(
            KnxObject knxObject,
            KnxObject? knxFeedbackObject)
            : base(HomeAutomationObjectType.Light, knxObject, knxFeedbackObject) { }
    }

    public static class MapLightStatusExtension
    {
        public static string MapLightStatus(this Light light)
            => light.Status switch
            {
                LightStatus.Unknown => "unknown",
                LightStatus.On => "on",
                LightStatus.Off => "off",
                _ => throw new ArgumentOutOfRangeException(),
            };
    }
}
