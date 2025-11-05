using MassTransit;
using AuthService.Core.Sagas;
using CoreLib.Events.TransactionEvents;

namespace AuthService.Core.Sagas;

public class TransactionCoordinatorSaga : MassTransitStateMachine<TransactionSagaState>
{
    // States - объявляем как свойства
    public State CreatingTransaction { get; set; } = null!;
    public State UpdatingWallet { get; set; } = null!;
    public State UpdatingStatistics { get; set; } = null!;
    public State Completed { get; set; } = null!;

    // Events
    public Event<CreateTransaction> TransactionCreated { get; set; } = null!;
    public Event<WalletBalanceUpdated> WalletUpdated { get; set; } = null!;
    public Event<StatisticsUpdated> StatisticsUpdated { get; set; } = null!;

    public TransactionCoordinatorSaga()
    {
        // Инициализация состояний через InstanceState
        InstanceState(x => x.CurrentState);

        // Настройка корреляции событий
        Event(() => TransactionCreated, e => e.CorrelateById(context => context.Message.TransactionId));
        Event(() => WalletUpdated, e => e.CorrelateById(context => context.Message.WalletId));
        Event(() => StatisticsUpdated, e => e.CorrelateById(context => context.Message.UserId));

        // Определение поведения
        Initially(
            When(TransactionCreated)
                .Then(context =>
                {
                    context.Saga.TransactionId = context.Message.TransactionId;
                    context.Saga.Amount = context.Message.Amount;
                    context.Saga.UserId = context.Message.UserId;
                    context.Saga.WalletId = context.Message.WalletId;
                })
                .TransitionTo(CreatingTransaction)
                .Publish(context => new TransactionCreated(
                    context.Message.TransactionId,
                    context.Message.UserId))
        );

        During(CreatingTransaction,
            When(WalletUpdated)
                .TransitionTo(UpdatingStatistics)
                .Publish(context => new StatisticsUpdated(context.Saga.UserId))
        );

        During(UpdatingStatistics,
            When(StatisticsUpdated)
                .TransitionTo(Completed)
                .Publish(context => new TransactionSagaCompleted(context.Saga.TransactionId))
                .Finalize()
        );
    }
}