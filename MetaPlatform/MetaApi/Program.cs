using MetaApi.AppStart;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using MetaApi.Configuration;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MetaApi.AppStart.Extensions;
using MetaApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

//string response = "{\"response\":[{\"id\":884910363,\"first_name\":\"Vladimir\",\"last_name\":\"Dezhurnyuk\",\"can_access_closed\":true,\"is_closed\":false}]}";
//var userResponse = JsonSerializer.Deserialize<VkUserResponse>(response);

var startup = new Startup();
startup.Initialize(builder);

var app = builder.Build();

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var dbInfo = builder.Configuration.GetConnectionString(DatabaseConfig.MetaDbMsSqlConnection);
//var rootPath = builder.Environment.ContentRootPath;
StringBuilder sb = new StringBuilder();
sb.AppendLine($"Environment1: {environment}");
sb.AppendLine($"dbInfo1: {dbInfo}");
//sb.AppendLine($"rootPath: {rootPath}");
app.Logger.LogInformation(sb.ToString());

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.ApplyCors();

app.UseRouting();
app.UseAuthorization();
app.ApplyRateLimit();

app.ApplyMigrations();

app.ApplyAllHealthChecks();

// ��� ��������� middleware
app.UseMiddleware<TryOnLimitMiddleware>();

// ���������� ��������� ����������� ������
app.UseStaticFiles();

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
