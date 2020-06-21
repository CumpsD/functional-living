namespace FunctionalLiving.Infrastructure.Toggles
{
    using FeatureToggle;

    public class SendToSignalR : IFeatureToggle
    {
        public const string ConfigurationPath = "Features:SendToSignalR";

        public bool FeatureEnabled { get; }

        public SendToSignalR(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }
}
