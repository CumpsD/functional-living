namespace FunctionalLiving.Tests
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.EventHandling.Autofac;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing.SqlStreamStore.Autofac;
    using Infrastructure.Modules;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Xunit.Abstractions;

    public class FunctionalLivingTest : AutofacBasedTest
    {
        private readonly JsonSerializerSettings _eventSerializerSettings = EventsJsonSerializerSettingsProvider.CreateSerializerSettings();
        private readonly IConfigurationRoot _configuration;

        public FunctionalLivingTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            var builder = new ConfigurationBuilder();

            builder.AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ConnectionStrings:Events", "DummyConnection" },
            });

            _configuration = builder.Build();
        }

        protected override void ConfigureCommandHandling(ContainerBuilder builder)
            => builder.RegisterModule(new CommandHandlingModule(_configuration));

        protected override void ConfigureEventHandling(ContainerBuilder builder)
            => builder.RegisterModule(new EventHandlingModule(typeof(DomainAssemblyMarker).Assembly, _eventSerializerSettings));
    }
}
