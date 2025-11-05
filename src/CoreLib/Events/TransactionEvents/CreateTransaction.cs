namespace CoreLib.Events.TransactionEvents;

public record CreateTransaction(
    Guid TransactionId, 
    decimal Amount, 
    string Description, 
    int Type, 
    Guid WalletId, 
    Guid UserId
);
