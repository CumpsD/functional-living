namespace FunctionalLiving.Knx.Sender.Infrastructure.Toggles
{
    using FeatureToggle;

    public class DebugKnxCemi : IFeatureToggle
    {
        public const string ConfigurationPath = "Features:DebugKnxCemi";

        public bool FeatureEnabled { get; }

        public DebugKnxCemi(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }
}
