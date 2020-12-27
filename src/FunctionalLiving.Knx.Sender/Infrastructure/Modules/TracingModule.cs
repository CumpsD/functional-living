namespace FunctionalLiving.Knx.Sender.Infrastructure.Modules
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Autofac;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using OpenTelemetry.Trace;

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
                .AddOpenTelemetryTracing(builder => builder
                    .AddAspNetCoreInstrumentation()
                    .AddJaegerExporter(jaeger =>
                    {
                        jaeger.AgentHost = configuration.GetValue<string>("Tracing:Host");
                        jaeger.AgentPort = configuration.GetValue<int>("Tracing:Port");
                        jaeger.ProcessTags = new List<KeyValuePair<string, object>>
                        {
                            new KeyValuePair<string, object>("ServiceName", configuration.GetValue<string>("Tracing:ServiceName"))
                        };
                    })
                    .SetSampler(new AlwaysOnSampler()));

            //services
            //    .AddOpenTelemetryTracing(builder => builder
            //        .AddDependencyAdapter()
        }
    }
}
