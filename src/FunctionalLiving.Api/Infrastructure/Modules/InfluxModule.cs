namespace FunctionalLiving.Api.Infrastructure.Modules
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Autofac;
    using FeatureToggle;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using InfluxDB.Client;

    public class InfluxModule : Module
    {
        private readonly IConfiguration _configuration;

        public InfluxModule(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            var influxEndpoint = _configuration.GetValue<Uri>("InfluxDb:Endpoint");
            var token = _configuration.GetValue<string>("InfluxDb:Token");
            var bucket = _configuration.GetValue<string>("InfluxDb:Bucket");
            var org = _configuration.GetValue<string>("InfluxDb:Organisation");
            var logLevel = _configuration.GetValue<InfluxDB.Client.Core.LogLevel>("InfluxDb:LogLevel");

            // None = 0,
            // Basic = 1,
            // Headers = 2,
            // Body = 3

            // TODO: What is the usage pattern for influxdbclient?
            containerBuilder
                .Register(context =>
                {
                    var influxDbClientOptions = InfluxDBClientOptions
                        .Builder
                        .CreateNew()
                        .Url(influxEndpoint.ToString())
                        .AuthenticateToken(token.ToCharArray())
                        .Org(org)
                        .Bucket(bucket)
                        .LogLevel(logLevel)
                        .AddDefaultTag("application", "FunctionalLiving.Api")
                        .Build();

                    return InfluxDBClientFactory.Create(influxDbClientOptions);
                })
                .SingleInstance()
                .As<InfluxDBClient>();

            containerBuilder
                .Register<Func<WriteApi>>(context =>
                {
                    var influxDbClient = context.Resolve<InfluxDBClient>();

                    var writeOptions = WriteOptions
                        .CreateNew()
                        .BatchSize(5000)
                        .FlushInterval(1000)
                        .JitterInterval(1000)
                        .RetryInterval(5000)
                        .Build();

                    return () => influxDbClient.GetWriteApi(writeOptions);
                })
                .As<Func<WriteApi>>();
        }
    }
}
