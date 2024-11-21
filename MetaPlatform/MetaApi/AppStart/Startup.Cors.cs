namespace MetaApi.AppStart
{
    public partial class Startup
    {
        void ConfigureCors()
        {
            _builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()  // Разрешить любой источник
                       .AllowAnyMethod()  // Разрешить любые HTTP-методы (GET, POST, PUT и т. д.)
                       .AllowAnyHeader(); // Разрешить любые заголовки
                });
                    

                    //builder.WithOrigins("https://apidocs3.scout-gps.ru")
                    //    .AllowAnyHeader()
                    //    .AllowAnyMethod();
                    //builder.WithOrigins("https://apidocs.scout-gps.ru")
                    //    .AllowAnyHeader()
                    //    .AllowAnyMethod();
               
            });
        }
    }
}
