using AuthService.Core.Infrastructure;
using AuthService.Core.Sagas;
using AuthService.Api.Middleware;
using AuthService.Api.Consumers;
using AuthService.DAL.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// MassTransit configuration
builder.Services.AddMassTransit(x =>
{
    // x.AddSagaStateMachine<TransactionCoordinatorSaga, TransactionSagaState>()
    //  .InMemoryRepository();
    
    x.AddConsumer<TransactionCreatedConsumer>();
    
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlite("Data Source=authservice.db"));

// TraceId и HttpService
builder.Services.AddScoped<TraceService>();
builder.Services.AddHttpClient<HttpService>(client => 
{
    client.BaseAddress = new Uri("http://localhost:5101/");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseMiddleware<TraceIdMiddleware>();
app.MapControllers();

app.Run();