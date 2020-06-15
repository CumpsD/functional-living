namespace FunctionalLiving.Knx.Sender.Infrastructure.Toggles
{
    using FeatureToggle;

    public class SendToApi : IFeatureToggle
    {
        public const string ConfigurationPath = "Features:SendToApi";

        public bool FeatureEnabled { get; }

        public SendToApi(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }
}
