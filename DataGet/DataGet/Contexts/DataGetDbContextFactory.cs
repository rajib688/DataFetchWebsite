using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DataGet.Contexts
{
    public class DataGetDbContextFactory : IDesignTimeDbContextFactory<DataGetDbContext>
    {
        public DataGetDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<DataGetDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new DataGetDbContext(optionsBuilder.Options);
        }
    }
}
