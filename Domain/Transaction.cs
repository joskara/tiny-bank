using Infrastructure.Enums;

namespace Domain;

public class Transaction
{
    public Transaction(
        Guid id,
        Guid? originAccountId,
        Guid? destinationAccountId,
        decimal amount,
        TransactionType transactionType,
        TransactionStatus status,
        DateTime dateCreated,
        Guid createdBy)
    {
        if (id == Guid.Empty || id == default)
        {
            Id = Guid.NewGuid();
            DateCreated = DateTime.UtcNow;
        }
        else
        {
            Id = id;
            DateCreated = dateCreated;
        }
        
        OriginAccountId = originAccountId;
        DestinationAccountId = destinationAccountId;
        Amount = amount;
        Type = transactionType;
        Status = status;
        DateCreated = dateCreated;
        CreatedBy = createdBy;
    }
    
    public Guid Id { get; private set; }
    
    public Guid? OriginAccountId { get; private set; }
    
    public Guid? DestinationAccountId { get; private set; }
    
    public decimal Amount { get; private set; }
    
    public TransactionType Type { get; private set; }
    
    public TransactionStatus Status { get; private set; }
    
    public DateTime DateCreated { get; private set; }
    
    public Guid CreatedBy { get; private set; }

    public void UpdateStatus(TransactionStatus status)
    {
        Status = status;
    }
}