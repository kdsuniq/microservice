using CoreLib.Events.TransactionEvents;
using MassTransit;

namespace Expenses.Api.Consumers;

public class WalletBalanceUpdatedConsumer : IConsumer<WalletBalanceUpdated>
{
    private readonly ILogger<WalletBalanceUpdatedConsumer> _logger;

    public WalletBalanceUpdatedConsumer(ILogger<WalletBalanceUpdatedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<WalletBalanceUpdated> context)
    {
        _logger.LogInformation("WalletBalanceUpdatedConsumer: Баланс кошелька {WalletId} обновлен до {NewBalance}", 
            context.Message.WalletId, context.Message.NewBalance);
        
        await Task.Delay(100);
    }
}