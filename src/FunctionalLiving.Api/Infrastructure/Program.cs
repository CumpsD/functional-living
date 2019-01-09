namespace FunctionalLiving.Api.Infrastructure
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.AspNetCore.Hosting;
    using Be.Vlaanderen.Basisregisters.Api;

    public class Program
    {
        private static readonly Tuple<string, string> DevelopmentCertificate = new Tuple<string, string>(
            "example.pfx",
            "example-registry!");

        public static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
            => new WebHostBuilder()
                .UseDefaultForApi<Startup>(
                    httpPort: 1090,
                    httpsPort: 1444,
                    httpsCertificate: () => new X509Certificate2(DevelopmentCertificate.Item1, DevelopmentCertificate.Item2),
                    commandLineArgs: args);
    }
}
