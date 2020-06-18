namespace FunctionalLiving.Light.Commands
{
    using ValueObjects;

    public class TurnOnLightCommand
    {
        public LightId LightId { get; }

        public TurnOnLightCommand(LightId lightId)
            => LightId = lightId;
    }
}
