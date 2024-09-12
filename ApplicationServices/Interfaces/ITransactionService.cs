using Domain;

namespace ApplicationServices.Interfaces;

public interface ITransactionService
{
    public IEnumerable<Transaction> GetTransactions();
    
    public Transaction? GetTransaction(Guid transaction);
    
    public Guid AddTransaction(Transaction transaction);
}