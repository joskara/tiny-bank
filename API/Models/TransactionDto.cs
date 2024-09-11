using Infrastructure.Enums;

namespace API.Models;

public class TransactionDto
{
    public Guid Id { get; set; }
    
    public Guid OriginAccountId { get; set; }
    
    public Guid DestinationAccountId { get; set; }
    
    public decimal Amount { get; set; }
    
    public TransactionType Type { get; set; }
    
    public TransactionStatus Status { get; set; }
    
    public DateTime DateCreated { get; set; }
    
    public DateTime CreatedBy { get; set; }
}