using CoreLib.Events.TransactionEvents;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionSagaController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<TransactionSagaController> _logger;

    public TransactionSagaController(IPublishEndpoint publishEndpoint, ILogger<TransactionSagaController> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    [HttpPost("coordinator")]
    public async Task<IActionResult> CreateTransactionCoordinator()
    {
        var transactionId = Guid.NewGuid();
        var walletId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        _logger.LogInformation("Запуск Coordinator SAGA для транзакции {TransactionId}", transactionId);
        
        await _publishEndpoint.Publish(new CreateTransaction(
            transactionId, 
            1500.0m, 
            "Покупка продуктов", 
            1, // Expense
            walletId, 
            userId));
            
        return Ok(new { 
            TransactionId = transactionId, 
            Type = "Coordinator",
            Message = "SAGA запущена через Coordinator" 
        });
    }

    [HttpPost("orchestrator")]
    public async Task<IActionResult> CreateTransactionOrchestrator()
    {
        var transactionId = Guid.NewGuid();
        var walletId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        _logger.LogInformation("Запуск Orchestrator для транзакции {TransactionId}", transactionId);
        
        await _publishEndpoint.Publish(new CreateTransaction(
            transactionId, 
            2000.0m, 
            "Зарплата", 
            0, // Income
            walletId, 
            userId));
            
        return Ok(new { 
            TransactionId = transactionId, 
            Type = "Orchestrator",
            Message = "SAGA запущена через Orchestrator" 
        });
    }
}