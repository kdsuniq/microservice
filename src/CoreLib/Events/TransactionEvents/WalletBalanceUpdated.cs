namespace CoreLib.Events.TransactionEvents;

public record WalletBalanceUpdated(
    Guid WalletId, 
    Guid UserId, 
    decimal NewBalance
);
