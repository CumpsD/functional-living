namespace FunctionalLiving.Infrastructure.Toggles
{
    using FeatureToggle;

    public class SendToKnxSender : IFeatureToggle
    {
        public const string ConfigurationPath = "Features:SendToKnxSender";

        public bool FeatureEnabled { get; }

        public SendToKnxSender(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }
}
