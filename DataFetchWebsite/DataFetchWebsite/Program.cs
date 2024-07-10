using Autofac;
using Autofac.Extensions.DependencyInjection;
using DataFetchWebsite.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using System;

namespace DataFetchWebsite
{
    public class Program
    {
        private static string _connectionString;
        private static string _migrationAssemblyName;
        private static IConfiguration _configuration;

        public static void Main(string[] args)
        {
            try
            {
                _configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load configuration from file 'appsettings.json': {ex.Message}");
                Log.Fatal(ex, "Failed to load configuration");
                return;
            }

            _connectionString = _configuration.GetConnectionString("DefaultConnection");
            _migrationAssemblyName = typeof(Program).Assembly.FullName;

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ArgumentNullException(nameof(_connectionString), "Connection string 'DefaultConnection' not found.");
            }

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
            .ConfigureServices(services =>
            {  
                services.AddDbContext<DataFetchDbContext>(options =>
                    options.UseSqlServer("Server = DESKTOP - K32T5PF; Database = DataFetchWebsite; User Id = sa; Password = Rajib@2024; Trust Server Certificate = True", b => b.MigrationsAssembly(_migrationAssemblyName)));

                services.AddHostedService<Worker>();

            });
    }
}

