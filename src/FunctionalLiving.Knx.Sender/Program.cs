namespace FunctionalLiving.Knx.Sender
{
    using System;
    using System.IO;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        private static IConfigurationRoot _configuration;

        public static void Main(string[] args)
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
                .Build();
        }
    }
}
