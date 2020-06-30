namespace FunctionalLiving.Knx.Listener.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;

    internal class WriteTo
    {
        public string Name { get; set; }

        public Dictionary<string, string> Args { get; set; }

        public WriteTo()
        {
            Args = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }
    }

    internal static class WaitFor
    {
        public static async Task SeqToBecomeAvailable(
            IConfiguration configuration,
            CancellationToken token = default)
        {
            var writeTos = configuration
                .GetSection("Serilog:WriteTo")
                .Get<WriteTo[]>();

            var seq = writeTos.SingleOrDefault(to => "Seq".Equals(to.Name, StringComparison.InvariantCultureIgnoreCase));
            if (seq == null)
                return;

            using (var client = new HttpClient { BaseAddress = new Uri(seq.Args["ServerUrl"]) })
            {
                var exit = false;
                while (!exit)
                {
                    Console.WriteLine("Waiting for Seq to become available...");

                    try
                    {
                        using (var response = await client.GetAsync("/api", token))
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                await Task.Delay(TimeSpan.FromSeconds(1), token);
                            }
                            else
                            {
                                exit = true;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(
                            "Observed an exception while waiting for Seq to become available. Exception: {0}",
                            exception);

                        await Task.Delay(TimeSpan.FromSeconds(1), token);
                    }
                }
            }

            Console.WriteLine("Seq became available.");
        }
    }
}
