using Microsoft.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;

namespace yocar.Insurance.Data;

public class InsuranceEFCoreDbSchemaMigrator : ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public InsuranceEFCoreDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the InsuranceDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<InsuranceDbContext>()
            .Database
            .MigrateAsync();
    }
}
