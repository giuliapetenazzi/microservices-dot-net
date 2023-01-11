using CommandsService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CommandsService.EventProcessing;
using CommandsService.AsyncDataServices;
using CommandsService.SyncDataServices.Grpc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase(databaseName:"InMen")
);
builder.Services.AddScoped<ICommandRepo, CommandRepo>();
builder.Services.AddControllers();
builder.Services.AddHostedService<MessageBusSubscriber>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IPlatformDataClient, PlatformDataClient>();


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

PrepDb.PrepPopulation(app);

app.Run();
