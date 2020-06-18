namespace FunctionalLiving.Api.Infrastructure.Toggles
{
    using FeatureToggle;

    public class SendToInflux : IFeatureToggle
    {
        public const string ConfigurationPath = "Features:SendToInflux";

        public bool FeatureEnabled { get; }

        public SendToInflux(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }
}
