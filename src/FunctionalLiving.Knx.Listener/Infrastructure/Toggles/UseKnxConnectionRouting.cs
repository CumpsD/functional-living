namespace FunctionalLiving.Knx.Listener.Infrastructure.Toggles
{
    using FeatureToggle;

    public class UseKnxConnectionRouting : IFeatureToggle
    {
        public const string ConfigurationPath = "Features:UseKnxConnectionRouting";

        public bool FeatureEnabled { get; }

        public UseKnxConnectionRouting(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }
}
