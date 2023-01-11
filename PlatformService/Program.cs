using PlatformService.Data;
using Microsoft.EntityFrameworkCore;
using PlatformService.SyncDataServices.Http;
using Microsoft.Extensions.Configuration;
using PlatformService.AsyncDataServices;
using PlatformService.SyncDataServices.Grpc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsProduction())
{
    Console.WriteLine("--> Using SqlServer Db...", builder.Configuration["PlatformsConn"]);
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn"))
    );

} else {
    Console.WriteLine("--> Using InMem Db...");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseInMemoryDatabase(databaseName:"InMem")
    );
}


builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddGrpc();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandService"]}");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseRouting();
    app.UseAuthorization();
    app.UseEndpoints(endpoints => {
        endpoints.MapControllers();
    });
}


app.UseHttpsRedirection();

app.MapControllers();
app.MapGrpcService<GrpcPlatformService>();

app.MapGet("/protos/platforms.proto", async context =>
    {
        await context.Response.WriteAsync(System.IO.File.ReadAllText("Protos/platforms.proto"));
    }
);

PrepDb.PrepPopulation(app, app.Environment.IsProduction());
Console.WriteLine("--> Prepared Population...");

app.Run();
