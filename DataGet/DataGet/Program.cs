using DataGet;
using Serilog.Events;
using Serilog;
using Microsoft.EntityFrameworkCore;
using Autofac.Extensions.DependencyInjection;
using DataGet.Contexts;

namespace DataFetchWebsite
{
    public class Program
    {
        public static IConfiguration _configuration;

        public static void Main(string[] args)
        {
            try
            {
                _configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load configuration from file 'appsettings.json': {ex.Message}");
                Log.Fatal(ex, "Failed to load configuration");
                return;
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(_configuration)
                .CreateLogger();

            try
            {
                Log.Information("Web Service successfully started up.");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Web Service start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .UseSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<DataGetDbContext>(options =>
                    options.UseSqlServer(connectionString));
                services.AddHttpClient();
                services.AddHostedService<Worker>();
            });
    }
}