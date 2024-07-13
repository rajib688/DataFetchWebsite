using DataFetchWebsite.Entities;
using Microsoft.EntityFrameworkCore;


namespace DataFetchWebsite.Contexts
{
    public class DataFetchDbContext : DbContext
    {
        private readonly string _connectionString = "Server=DESKTOP-K32T5PF;Database=DataFetchWebsite;User Id=sa;Password=Rajib@2024;Trust Server Certificate=True";
        private readonly string _migrationAssemblyName;
        public DataFetchDbContext(DbContextOptions<DataFetchDbContext> options) : base(options) { }
        public DataFetchDbContext(string connectionString)
        {
            _connectionString = connectionString;         
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
            //if (!optionsBuilder.IsConfigured)
            //{
            //    optionsBuilder.UseSqlServer(_connectionString);
            //}
            //base.OnConfiguring(optionsBuilder);
        }

        public DbSet<WebsiteData> WebsiteDatas { get; set; }

    }
}