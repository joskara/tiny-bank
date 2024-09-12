namespace ApplicationServices.Interfaces;

public interface ITransactionStrategy
{
    void ExecuteTransaction();
    
    void ValidateTransaction();
}