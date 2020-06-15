namespace FunctionalLiving.Knx.Sender.Infrastructure.Toggles
{
    using FeatureToggle;

    public class SendToLog : IFeatureToggle
    {
        public const string ConfigurationPath = "Features:SendToLog";

        public bool FeatureEnabled { get; }

        public SendToLog(bool featureEnabled) => FeatureEnabled = featureEnabled;
    }
}
