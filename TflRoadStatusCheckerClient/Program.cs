using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TflRoadStatusCheckerClient.Contract;
using TflRoadStatusCheckerClient.Contract.Models;
using TflRoadStatusCheckerClient.Service;

namespace TflRoadStatusCheckerClient
{
    internal class Program
    {
        private const string EnvironmentVariablePrefix = "TFL_ROADSTATUS_CHECKER_";

        public static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine($"No argument passed. Please consider to pass a RoadName value..");
            }
            var host = new HostBuilder()
                .ConfigureHostConfiguration(hostConfig =>
                {
                    hostConfig.SetBasePath(Directory.GetCurrentDirectory());
                    hostConfig.AddJsonFile("hostsettings.json", true);
                    hostConfig.AddEnvironmentVariables(EnvironmentVariablePrefix);
                    hostConfig.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath);
                    config.AddJsonFile("appsettings.json", true, true);
                    config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json");
                    config.AddCommandLine(args);
                })
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddSingleton(new CommandLineArgs { Args = args[0] });
                    var baseUrls = new TflBaseUrls();
                    hostingContext.Configuration.GetSection("TflBaseUrls").Bind(baseUrls);
                    services.Configure<TflRoadStatusCredentials>(options =>
                        hostingContext.Configuration.GetSection("TflRoadStatusCredentials").Bind(options));

                    services.AddHttpClient<ITflRoadStatusCheckerAPIService<TflRoadStatusCheckerResponse>, TflRoadStatusCheckerAPIService>(client =>
                    {
                        client.BaseAddress = new Uri(baseUrls.RoadStatusChecker);
                        client.DefaultRequestHeaders
                            .Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    });
                    services.AddHostedService<ApiProcessingService>();
                    services.AddLogging();
                }
                )
                .Build();
            await host.RunAsync();
        }
    }
}