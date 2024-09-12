namespace Domain;

public class Account
{
    public Account(
        Guid id,
        Guid userId,
        decimal balance,
        bool isActive,
        DateTime createdAt,
        DateTime modifiedAt)
    {
        if (id.Equals(Guid.Empty) || id.Equals(default))
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;;
        }
        else
        {
            Id = id;
            CreatedAt = createdAt;
            ModifiedAt = modifiedAt;
        }
        
        UserId = userId;
        Balance = balance;
        IsActive = isActive;
    }
    
    public Guid Id { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public decimal Balance { get; private set; }
    
    public bool IsActive { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public DateTime ModifiedAt { get; private set; }

    public void UpdateIsActive(bool isActive)
    {
        if (IsActive.Equals(isActive)) return;
        
        IsActive = isActive;
        ModifiedAt = DateTime.UtcNow;
    }

    public void Deposit(decimal transactionAmount)
    {
        Balance += transactionAmount;
        ModifiedAt = DateTime.UtcNow;
    }
    
    public void Withdrawal(decimal transactionAmount)
    {
        Balance -= transactionAmount;
        ModifiedAt = DateTime.UtcNow;
    }
}