using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GenderHealthCare.Repositories.Base
{
    public class GenderHealthCareDbContextFactory : IDesignTimeDbContextFactory<GenderHealthCareDbContext>
    {
        public GenderHealthCareDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                        .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../GenderHealthCare"))
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();


            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<GenderHealthCareDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new GenderHealthCareDbContext(optionsBuilder.Options);
        }
    }
}