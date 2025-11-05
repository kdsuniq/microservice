namespace CoreLib.Events.TransactionEvents;

public record TransactionCreated(
    Guid TransactionId, 
    Guid UserId
);
