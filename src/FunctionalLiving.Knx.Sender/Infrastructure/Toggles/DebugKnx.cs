namespace FunctionalLiving.Knx.Sender.Infrastructure.Toggles
{
    using FeatureToggle;

    public class DebugKnx : IFeatureToggle
    {
        public const string ConfigurationPath = "Features:DebugKnx";

        public bool FeatureEnabled { get; }

        public DebugKnx(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }
}
