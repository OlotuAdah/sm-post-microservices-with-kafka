using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Post.Query.Infrastructure.DataAccess;
public class ReadDatabaseContextFactory(Action<DbContextOptionsBuilder> configureDbContext) : IDesignTimeDbContextFactory<ReadDatabaseContext>
{
    private readonly Action<DbContextOptionsBuilder> _configureDbContext = configureDbContext;
    public ReadDatabaseContext CreateDbContext(string[] args = null)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ReadDatabaseContext>();
        _configureDbContext(optionsBuilder);
        return new ReadDatabaseContext(optionsBuilder.Options);
    }
}
// This factory is used to create an instance of the ReadDatabaseContext for design-time services such as migrations.
// It configures the context to use SQL Server with a connection string.