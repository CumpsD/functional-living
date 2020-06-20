namespace FunctionalLiving.Knx.Sender.Infrastructure.Modules
{
    using System.Diagnostics;
    using Autofac;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using OpenTelemetry.Trace.Configuration;
    using OpenTelemetry.Trace.Samplers;

    public class TracingModule : Module
    {
        public TracingModule(
            IConfiguration configuration,
            IServiceCollection services)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;

            var enableTracing = configuration.GetValue<bool>("Features:EnableTracing");
            if (!enableTracing)
                return;

            services
                .AddOpenTelemetry(builder => builder
                    .UseJaeger(jaeger =>
                    {
                        jaeger.AgentHost = configuration.GetValue<string>("Tracing:Host");
                        jaeger.AgentPort = configuration.GetValue<int>("Tracing:Port");
                        jaeger.ServiceName = configuration.GetValue<string>("Tracing:ServiceName");
                    })
                    .AddRequestAdapter()
                    .AddDependencyAdapter()
                    .SetSampler(new AlwaysOnSampler()));
        }
    }
}
