using CoreLib.Events.TransactionEvents;
using MassTransit;

namespace Expenses.Api.Consumers;

public class StatisticsUpdatedConsumer : IConsumer<StatisticsUpdated>
{
    private readonly ILogger<StatisticsUpdatedConsumer> _logger;

    public StatisticsUpdatedConsumer(ILogger<StatisticsUpdatedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<StatisticsUpdated> context)
    {
        _logger.LogInformation("StatisticsUpdatedConsumer: Обновляем статистику для пользователя {UserId}", 
            context.Message.UserId);
        
        await Task.Delay(100);
    }
}