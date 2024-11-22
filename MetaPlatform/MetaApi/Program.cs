using MetaApi.AppStart;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup();
startup.Initialize(builder);


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

// ���������� ��������� ����������� ������
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
