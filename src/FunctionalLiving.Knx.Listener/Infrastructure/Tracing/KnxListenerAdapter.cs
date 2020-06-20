namespace FunctionalLiving.Knx.Listener.Infrastructure.Tracing
{
    using System;
    using OpenTelemetry.Adapter;
    using OpenTelemetry.Trace;

    public class KnxListenerAdapter : IDisposable
    {
        private readonly DiagnosticSourceSubscriber _diagnosticSourceSubscriber;

        public KnxListenerAdapter(Tracer tracer)
        {
            _diagnosticSourceSubscriber = new DiagnosticSourceSubscriber(
                new SendKnxMessageListener(TracingConstants.ActivityName, tracer),
                null);

            _diagnosticSourceSubscriber.Subscribe();
        }

        public void Dispose()
            => _diagnosticSourceSubscriber?.Dispose();
    }
}
