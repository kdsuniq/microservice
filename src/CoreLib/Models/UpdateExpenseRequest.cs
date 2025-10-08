namespace CoreLib.Models;

public class UpdateExpenseRequest
{
    public Guid UserId { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
}