using MetaApi.AppStart;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using MetaApi.Configuration;
using System.Text;
using MetaApi.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup();
startup.Initialize(builder);

var app = builder.Build();

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var virtualFitConfig = builder.Configuration.GetSection(VirtualFitConfig.SectionName).Get<VirtualFitConfig>();
var dbInfo = builder.Configuration.GetConnectionString(DatabaseConfig.MetaDbMsSqlConnection);
var rootPath = builder.Environment.ContentRootPath;
StringBuilder sb = new StringBuilder();
sb.AppendLine($"Environment: {environment}");
sb.AppendLine($"config: {virtualFitConfig.ApiToken}");
sb.AppendLine($"dbInfo: {dbInfo}");
sb.AppendLine($"rootPath: {rootPath}");
app.Logger.LogInformation(sb.ToString());

app.UseSwagger();
app.UseSwaggerUI();

// Применение миграций автоматически при старте приложения
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MetaDbContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

// Добавление поддержки статических файлов
app.UseStaticFiles();

if (builder.Environment.IsDevelopment())
{
    //app.UseCors("AllowAll");
    app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
}
else
{
    //app.UseCors("AllowAll");
    app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
}

app.MapControllers();

OnApplicationStartLogAddresses(app);

app.Run();

void OnApplicationStartLogAddresses(WebApplication app)
{
    app.Lifetime.ApplicationStarted.Register(() =>
    {        
        var server = app.Services.GetRequiredService<IServer>();
        var addressesFeature = server.Features.Get<IServerAddressesFeature>();
        var addresses = addressesFeature?.Addresses;

        if (addresses != null)
        {
            foreach (var address in addresses)
            {
                app.Logger.LogInformation("Application is listening on address: {address}", address);
            }
        }
    });
}
