using Expenses.DAL.Data;
using Expenses.Logic.Services;
using Expenses.Api.Infrastructure;
using Expenses.Api.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов в контейнер
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SQLite база данных
builder.Services.AddDbContext<ExpensesDbContext>(options =>
    options.UseSqlite("Data Source=expenses.db"));

// Добавление сервисов бизнес-логики
builder.Services.AddScoped<ExpenseService>();

// Добавляем TraceService для работы с TraceId
builder.Services.AddScoped<TraceService>();

var app = builder.Build();

// Настройка конвейера HTTP запросов
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Добавляем TraceId middleware ПЕРЕД другими middleware
app.UseMiddleware<TraceIdMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Инициализация базы данных
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ExpensesDbContext>();
        
        // Создаем базу и таблицы
        await context.Database.EnsureCreatedAsync();

        // Добавление тестовых категорий, если их нет
        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new CoreLib.Models.Category { Id = Guid.NewGuid(), Name = "Food" },
                new CoreLib.Models.Category { Id = Guid.NewGuid(), Name = "Transport" },
                new CoreLib.Models.Category { Id = Guid.NewGuid(), Name = "Study" },
                new CoreLib.Models.Category { Id = Guid.NewGuid(), Name = "Entertainment" }
            );
            await context.SaveChangesAsync();
            Console.WriteLine("База данных инициализирована с тестовыми данными");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка инициализации БД: {ex.Message}");
}

app.Run();