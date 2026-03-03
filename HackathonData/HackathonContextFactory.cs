using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HackathonData.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HackathonData
{
    public class HackathonContextFactory : IDesignTimeDbContextFactory<HackathonContext>
    {
        public HackathonContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<HackathonContext>();

            
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\MSSQLLocalDB;Database=HackathonDB;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new HackathonContext(optionsBuilder.Options);
        }
    }
}

