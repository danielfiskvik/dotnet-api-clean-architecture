using ModularApp.Modules.Workspace.Application;
using ModularApp.WebApi;
using ModularApp.WebApi.Configurations;
using ModularApp.Modules.Workspace.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddPersistenceModule(builder.Configuration)
    .AddWebApiModule()
    .AddApplicationModule();

builder.Services
    .AddControllers()
    // Adding NewtonsoftJson because reference loop must be set to ignore since we expose EF Core Domain with multiple reference through the API.
    .AddNewtonsoftJson(options => 
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

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

await app.InitializePersistenceModuleAsync(builder.Configuration);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Make the Program class public using a partial class declaration. If not then the Integration Tests will not work.
namespace ModularApp.WebApi
{
    public partial class Program { }
}