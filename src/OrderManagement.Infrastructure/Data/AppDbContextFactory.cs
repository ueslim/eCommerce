using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Define o SQLite local temporário apenas para geração das migrations em tempo de design
            optionsBuilder.UseSqlite("Data Source=OrderManagement_Design.db");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
