namespace FunctionalLiving.Knx.Listener.Infrastructure.Toggles
{
    using FeatureToggle;

    public class UseKnxConnectionTunneling : IFeatureToggle
    {
        public const string ConfigurationPath = "Features:UseKnxConnectionTunneling";

        public bool FeatureEnabled { get; }

        public UseKnxConnectionTunneling(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }
}
