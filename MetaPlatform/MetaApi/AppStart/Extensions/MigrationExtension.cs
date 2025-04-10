using MetaApi.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.AppStart.Extensions
{
    public static class MigrationExtension
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using MetaDbContext metaDbContext = scope.ServiceProvider.GetService<MetaDbContext>();
            metaDbContext.Database.Migrate();
        }
    }
}
