namespace FunctionalLiving.Infrastructure
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource.SqlStreamStore.Autofac;
    using Autofac;
    using Microsoft.Extensions.Configuration;

    public static class ContainerBuilderExtensions
    {
        public static void RegisterEventstreamModule(this ContainerBuilder builder, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Events");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ApplicationException("Missing 'Events' connectionstring.");

            builder.RegisterModule(new SqlStreamStoreModule(connectionString, Schema.Default));
        }
    }
}
