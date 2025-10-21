using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Core.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    [NotMapped]
    public List<Wallet> Wallets { get; set; } = new();
    [NotMapped]
    public List<Transaction> Transactions { get; set; } = new();
}