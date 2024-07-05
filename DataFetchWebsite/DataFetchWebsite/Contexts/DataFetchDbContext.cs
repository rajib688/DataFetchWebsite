using DataFetchWebsite.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFetchWebsite.Contexts
{
    public class DataFetchDbContext : DbContext
    {
        private readonly String _ConnectionString;
        private readonly string _MigrationAssemblyName;
        public DataFetchDbContext(DbContextOptions<DataFetchDbContext> options)
        : base(options)
        {
        }
        public DataFetchDbContext(string ConnectionString, string MigrationAssemblyName)
        {
            _ConnectionString = ConnectionString;
            _MigrationAssemblyName = MigrationAssemblyName;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            if (!dbContextOptionsBuilder.IsConfigured)
            {
                dbContextOptionsBuilder.UseSqlServer(
                    _ConnectionString,
                    m => m.MigrationsAssembly(_MigrationAssemblyName));
            }
            base.OnConfiguring(dbContextOptionsBuilder);
        }
        public DbSet<WebsiteData> WebsiteDatas { get; set; }
    }
}
