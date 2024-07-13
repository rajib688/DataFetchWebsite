using Autofac.Extensions.DependencyInjection;
using DataFetchWebsite.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

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
        private static (string connectionString, string migrationAssemblyName) GetConnectionNameAndAssemblyName()
        {
            var connectionStringName = "DefaultConnection";
            var connectionString = _configuration.GetConnectionString(connectionStringName);
            var migrationAssemblyName = typeof(Program).Assembly.FullName;
            return (connectionString, migrationAssemblyName);
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    var _connectionInfo = GetConnectionNameAndAssemblyName();
                    services.AddDbContext<DataFetchDbContext>(options =>
                        options.UseSqlServer(_connectionInfo.connectionString));

                    services.AddHttpClient();

                    services.AddHostedService<Worker>();
                });
    }
}
