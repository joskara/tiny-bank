namespace Data.POCOs;

public class AccountPoco
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public decimal Balance { get; set; }
    
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime ModifiedAt { get; set; }
}