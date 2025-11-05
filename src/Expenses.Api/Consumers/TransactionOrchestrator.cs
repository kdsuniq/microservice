using CoreLib.Events.TransactionEvents;
using MassTransit;

namespace Expenses.Api.Consumers;

public class TransactionOrchestrator : IConsumer<CreateTransaction>
{
    private readonly ILogger<TransactionOrchestrator> _logger;

    public TransactionOrchestrator(ILogger<TransactionOrchestrator> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CreateTransaction> context)
    {
        _logger.LogInformation("Orchestrator: Начинаем обработку транзакции {TransactionId}", 
            context.Message.TransactionId);
        
        // 1. Создаем транзакцию
        await context.Publish(new TransactionCreated(
            context.Message.TransactionId, 
            context.Message.UserId));
        
        // 2. Обновляем баланс кошелька
        await context.Publish(new WalletBalanceUpdated(
            context.Message.WalletId,
            context.Message.UserId,
            context.Message.Amount));
        
        // 3. Обновляем статистику
        await context.Publish(new StatisticsUpdated(context.Message.UserId));
        
        // 4. Отправляем уведомление
        await context.Publish(new NotificationSent(
            context.Message.UserId,
            $"Создана транзакция: {context.Message.Description} на сумму {context.Message.Amount}"));
        
        _logger.LogInformation("Orchestrator: Транзакция {TransactionId} завершена", 
            context.Message.TransactionId);
    }
}