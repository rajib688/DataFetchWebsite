using DataFetchWebsite.Entities;
using Microsoft.EntityFrameworkCore;


namespace DataFetchWebsite.Contexts
{
    public class DataFetchDbContext : DbContext
    {
        private static string _connectionString;
        private static string _migrationAssemblyName;

        public DataFetchDbContext(string connectionString, string migrationAssemblyName)
        {
            _connectionString = connectionString;
            _migrationAssemblyName = migrationAssemblyName;
        }

        public DataFetchDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString, m => m.MigrationsAssembly(_migrationAssemblyName));
            }
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<WebsiteData> WebsiteDatas { get; set; }

    }
}