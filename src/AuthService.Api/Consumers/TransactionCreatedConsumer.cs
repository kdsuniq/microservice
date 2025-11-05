using CoreLib.Events.TransactionEvents;
using MassTransit;

namespace AuthService.Api.Consumers;

public class TransactionCreatedConsumer : IConsumer<TransactionCreated>
{
    private readonly ILogger<TransactionCreatedConsumer> _logger;

    public TransactionCreatedConsumer(ILogger<TransactionCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TransactionCreated> context)
    {
        _logger.LogInformation("Транзакция создана: {TransactionId} для пользователя {UserId}", 
            context.Message.TransactionId, context.Message.UserId);
        
        // Симуляция обработки транзакции
        await Task.Delay(500);
        
        // Отправляем событие обновления баланса кошелька
        await context.Publish(new WalletBalanceUpdated(
            Guid.NewGuid(), // В реальном приложении брали бы из базы
            context.Message.UserId,
            1500.0m // Новый баланс
        ));
        
        _logger.LogInformation("Баланс кошелька обновлен для пользователя {UserId}", 
            context.Message.UserId);
    }
}