using MassTransit;

namespace AuthService.Core.Sagas;

public class TransactionSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string? CurrentState { get; set; } // Добавил nullable
    public Guid TransactionId { get; set; }
    public decimal Amount { get; set; }
    public Guid UserId { get; set; }
    public Guid WalletId { get; set; }
}