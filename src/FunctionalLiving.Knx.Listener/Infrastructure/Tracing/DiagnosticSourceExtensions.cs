namespace FunctionalLiving.Knx.Listener.Infrastructure.Tracing
{
    using System;
    using System.Diagnostics;

    public static class DiagnosticSourceExtensions
    {
        public static Activity StartActivity(
            this DiagnosticSource diagnosticSource,
            string operationName,
            string eventName,
            object payload)
        {
            var activity = new Activity(operationName);

            if (diagnosticSource.IsEnabled(eventName))
            {
                diagnosticSource.StartActivity(activity, payload);
            }
            else
            {
                activity.Start();
            }

            return activity;
        }

        public static void StopActivity(
            this DiagnosticSource diagnosticSource,
            Activity activity)
        {
            if (activity.Duration == TimeSpan.Zero)
                activity.SetEndTime(DateTime.UtcNow);

            diagnosticSource.StopActivity(activity, null);
        }
    }
}
