using AuthService.Core.Infrastructure;
using AuthService.Api.Middleware;
using AuthService.DAL.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
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