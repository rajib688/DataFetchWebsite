using Autofac;
using DataFetchWebsite.Contexts;
using DataFetchWebsite.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFetchWebsite
{
    public class WorkerModule : Module
    {
        private static string _connectionString;
        private static string _migrationAssemblyName;
        private readonly IConfiguration _configuration;

        public WorkerModule(string connectionString, string migrationAssemblyName, IConfiguration configuration)
        {
            _connectionString = connectionString;
            _migrationAssemblyName = migrationAssemblyName;
            _configuration = configuration;
        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<DataFetchDbContext>();
                optionsBuilder.UseSqlServer(_connectionString, b => b.MigrationsAssembly(_migrationAssemblyName));

                return new DataFetchDbContext(optionsBuilder.Options);
            }).AsSelf().InstancePerLifetimeScope();

            //builder.RegisterType<DataFetchDbContext>().AsSelf()
            //   .WithParameter("connectionString", _connectionString)
            //   .WithParameter("migrationAssemblyName", _migrationAssemblyName)
            //   .InstancePerLifetimeScope();

            //builder.RegisterType<DataFetchDbContext>().AsSelf()
            //    .InstancePerLifetimeScope();

            builder.RegisterType<WebsiteDataService>().As<IWebsiteDataService>()
                .InstancePerLifetimeScope();

            base.Load(builder);

        }
    }
}
