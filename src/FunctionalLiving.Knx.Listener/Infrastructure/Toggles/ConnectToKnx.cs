namespace FunctionalLiving.Knx.Listener.Infrastructure.Toggles
{
    using FeatureToggle;

    public class ConnectToKnx : IFeatureToggle
    {
        public const string ConfigurationPath = "Features:ConnectToKnx";

        public bool FeatureEnabled { get; }

        public ConnectToKnx(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }
}
