using MetaApi.AppStart;

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

app.UseCors("AllowAll");

app.MapControllers();

app.Run();
