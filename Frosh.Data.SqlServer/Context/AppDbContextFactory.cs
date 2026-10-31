using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Frosh.Data.SqlServer.Context;

namespace Frosh.Data.SqlServer;

public class AppDbContextFactory
    : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=localhost;Database=FroshDb;Trusted_Connection=True;TrustServerCertificate=True;");

        return new AppDbContext(optionsBuilder.Options);
    }
}