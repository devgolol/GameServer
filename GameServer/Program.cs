
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Serilog ����
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/gameserver-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            //Serilog ���
            builder.Host.UseSerilog();

            // Add services to the container.

            //Entity Framework + MySQL ����
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<GameDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            //Redis ĳ�� ����
            builder.Services.AddStackExchangeRedisCache(options =>
                options.Configuration = builder.Configuration.GetConnectionString("RedisConnection"));

            //SignalR ����
            builder.Services.AddSignalR();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            //SignalR Hub ���� (���߿� �߰��� ����)
            //app.MalpHub<GameHub>("/gamehub");

            app.Run();
        }
    }
}
