using Autofac;
using Autofac.Extensions.DependencyInjection;
using DataFetchWebsite.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

namespace DataFetchWebsite
{
    public class Program
    {
        private static string _connectionString;
        private static string _migrationAssemblyName;
        private static IConfiguration _configuration;

        public static void Main(string[] args)
        {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables()
                .Build();
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
            _migrationAssemblyName = typeof(Worker).Assembly.FullName;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(_configuration)
                .CreateLogger();

            try
            {
                Log.Information("Web Service successfully start up.");
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
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterModule(new WorkerModule(_connectionString, _migrationAssemblyName, _configuration));
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<DataFetchDbContext>(options =>
                options.UseSqlServer(_connectionString, b => b.MigrationsAssembly(_migrationAssemblyName)));
                //options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));

                services.AddHostedService<Worker>();

            });
    }
}

