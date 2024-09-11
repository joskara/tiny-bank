namespace Domain;

public class User
{
    public User(
        Guid id,
        string firstName,
        string lastName,
        string fiscalNumber,
        bool isActive,
        DateTime createdAt,
        DateTime modifiedAt)
    {
        if (id.Equals(Guid.Empty) || id.Equals(default))
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;
        }
        else
        {
            Id = id;
            CreatedAt = createdAt;
            ModifiedAt = modifiedAt;
        }

        FirstName = firstName;
        LastName = lastName;
        FiscalNumber = fiscalNumber;
        IsActive = isActive;
    }
    
    public Guid Id { get; }
    
    public string FirstName { get;  private set; }
    
    public string LastName { get; private set; }
    
    public string FiscalNumber { get; private set; }
    
    public bool IsActive { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public DateTime ModifiedAt { get; private set; }
    
    public void UpdateIsActive(bool isActive)
    {
        if (IsActive.Equals(isActive)) return;
        
        IsActive = isActive;
        ModifiedAt = DateTime.UtcNow;
    }
}