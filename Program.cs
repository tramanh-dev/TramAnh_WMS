using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TramAnh_WMS.Data;

namespace TramAnh_WMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "NguyenThiTramAnh_2123110496", 
                    Version = "v1",
                    Description = "Đồ án Quản lý kho (WMS) - API Documentation"
                });
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "NguyenThiTramAnh_2123110496 V1");
                c.RoutePrefix = string.Empty;
                c.DocumentTitle = "NguyenThiTramAnh_2123110496";
            });

            // app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }

            app.Run();
        }
    }
}