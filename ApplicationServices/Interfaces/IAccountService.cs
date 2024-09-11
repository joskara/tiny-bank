using Domain;

namespace ApplicationServices.Interfaces;

public interface IAccountService
{
    public IEnumerable<Account> GetAccounts();
    
    public Account? GetAccount(Guid accountId);
    
    public Guid AddAccount(Account account);
    
    public void UpdateAccountActive(Guid accountId, bool active);
}