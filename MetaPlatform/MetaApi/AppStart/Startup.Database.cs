using MetaApi.Configuration;
using MetaApi.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.AppStart
{
    public partial class Startup
    {
        void ConfigureDatabase(IConfiguration configuration)
        {
            _builder.Services.AddDbContext<MetaDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString(DatabaseConfig.MetaDbMsSqlConnection),
                                     sqlServerOptionsAction: sqlOption =>
                                     {
                                         sqlOption.EnableRetryOnFailure();
                                     });

                //options.UseNpgsql(configuration.GetConnectionString(DatabaseConfig.MetaDbMsSqlConnection));
            });
        }
    }
}
