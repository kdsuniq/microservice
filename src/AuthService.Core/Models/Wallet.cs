using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Core.Models;

public class Wallet
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "RUB";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
     
    public User? User { get; set; }
    [NotMapped]
    public List<Transaction> Transactions { get; set; } = new();
}