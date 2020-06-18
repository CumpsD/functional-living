namespace FunctionalLiving.Light.Commands
{
    using ValueObjects;

    public class TurnOffLightCommand
    {
        public LightId LightId { get; }

        public TurnOffLightCommand(LightId lightId)
            => LightId = lightId;
    }
}
