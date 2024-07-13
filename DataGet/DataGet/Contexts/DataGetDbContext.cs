using DataGet.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataGet.Contexts
{
    public class DataGetDbContext : DbContext
    {
        public DataGetDbContext(DbContextOptions<DataGetDbContext> options)
            : base(options)
        {
        }
        public DbSet<WebsiteData> websiteDatas { get; set; }
        public DbSet<Company> Companys { get; set; }
    }
}
