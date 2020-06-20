namespace FunctionalLiving.Knx.Sender.Infrastructure.Toggles
{
    using FeatureToggle;

    public class ConnectToKnx : IFeatureToggle
    {
        public const string ConfigurationPath = "Features:ConnectToKnx";

        public bool FeatureEnabled { get; }

        public ConnectToKnx(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }
}
