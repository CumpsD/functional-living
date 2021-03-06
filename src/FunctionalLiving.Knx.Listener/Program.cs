namespace FunctionalLiving.Knx.Listener
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Infrastructure;
    using Infrastructure.Modules;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using OpenTelemetry.Trace;
    using Serilog;

    public class Program
    {
        private static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        public static async Task Main(string[] args)
        {
            var ct = CancellationTokenSource.Token;

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                CancellationTokenSource.Cancel();
                eventArgs.Cancel = true;
            };

            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
                Log.Debug(
                    eventArgs.Exception,
                    "FirstChanceException event raised in {AppDomain}.",
                    AppDomain.CurrentDomain.FriendlyName);

            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
                Log.Fatal(
                    (Exception)eventArgs.ExceptionObject,
                    "Encountered a fatal exception, exiting program.");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .AddCommandLine(args ?? Array.Empty<string>())
                .Build();

            var container = ConfigureServices(configuration);
            //var openTelemetry = container.GetService<TracerFactoryBase>();
            var logger = container.GetRequiredService<ILogger<Program>>();

            try
            {
                await WaitFor.SeqToBecomeAvailable(configuration, ct).ConfigureAwait(false);

                logger.LogInformation("Starting FunctionalLiving.Knx.Listener");

                var runner = container.GetRequiredService<KnxListener>();

                runner.Start();

                logger.LogInformation("Running... Press CTRL + C to exit.");
                ct.WaitHandle.WaitOne();
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Encountered a fatal exception, exiting program.");
            }

            logger.LogInformation("Stopping...");
            Log.CloseAndFlush();

            ct.WaitHandle.Close();
        }

        private static IServiceProvider ConfigureServices(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            var builder = new ContainerBuilder();

            builder
                .RegisterModule(new LoggingModule(configuration, services));

            var tempProvider = services.BuildServiceProvider();
            var loggerFactory = tempProvider.GetRequiredService<ILoggerFactory>();

            services
                .Configure<KnxConfiguration>(configuration.GetSection(KnxConfiguration.ConfigurationPath));

            builder
                .RegisterModule(new TracingModule(configuration, services))
                .RegisterModule(new HttpModule(configuration, services, loggerFactory))
                .RegisterModule(new TogglesModule(configuration, loggerFactory));

            builder
                .RegisterType<KnxListener>()
                .SingleInstance();

            builder
                .Populate(services);

            return new AutofacServiceProvider(builder.Build());
        }
    }
}
