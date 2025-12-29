using LandValueScraper.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;

namespace LandValueScraper;

public sealed class Program
{
    public static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostBuilder, services) =>
            {
                services.AddHostedService<Scrape>();

                services.AddHttpClient<ScrapeLandValues>()
                    .AddTransientHttpErrorPolicy(policy => policy
                    .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)))); //exponential backoff

                services.AddSingleton<DeserializeAddressGeoJson>();
                services.AddSingleton<ParseClackamasData>();
                services.AddSingleton<DatasetSerializer>();
            }).Build();

        try
        {
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
