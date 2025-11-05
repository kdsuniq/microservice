namespace CoreLib.Events.TransactionEvents;

public record NotificationSent(
    Guid UserId, 
    string Message
);
