namespace FunctionalLiving.Knx.Listener.Infrastructure.Tracing
{
    using OpenTelemetry.Trace.Configuration;

    public static class TraceBuilderExtensions
    {
        public static TracerBuilder AddKnxListenerAdapter(this TracerBuilder builder)
            => builder.AddAdapter(t => new KnxListenerAdapter(t));
    }
}
