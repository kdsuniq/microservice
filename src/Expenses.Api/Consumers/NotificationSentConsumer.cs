using CoreLib.Events.TransactionEvents;
using MassTransit;

namespace Expenses.Api.Consumers;

public class NotificationSentConsumer : IConsumer<NotificationSent>
{
    private readonly ILogger<NotificationSentConsumer> _logger;

    public NotificationSentConsumer(ILogger<NotificationSentConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<NotificationSent> context)
    {
        _logger.LogInformation("NotificationSentConsumer: Отправляем уведомление: {Message}", 
            context.Message.Message);
        
        await Task.Delay(100);
    }
}