using PlatformService.Data;
using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;
using Microsoft.Extensions.Configuration;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
                     opt.UseInMemoryDatabase(
                        databaseName:"InMem"
                        ));
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandService"]}");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseRouting();
    app.UseEndpoints(endpoints => {
        endpoints.MapControllers();
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulation(app, app.Environment.IsProduction());
Console.WriteLine("--> Prepared Population...");

app.Run();
