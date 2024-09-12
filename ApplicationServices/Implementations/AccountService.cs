using System.ComponentModel.DataAnnotations;
using ApplicationServices.Interfaces;
using ApplicationServices.Mappers;
using Data.POCOs;
using Data.Repositories;
using Domain;

namespace ApplicationServices.Implementations;

public class AccountService(
    IBaseRepository<AccountPoco> accountRepository,
    IBaseRepository<UserPoco> userRepository) : IAccountService
{
    public IEnumerable<Account> GetAccounts()
    {
        var accountsPoco = accountRepository.Get();

        return accountsPoco.ToDomain();
    }

    public Account GetAccount(Guid accountId)
    {
        var accountPoco = accountRepository.GetById(accountId);

        return accountPoco.ToDomain();
    }

    public Guid AddAccount(Account account)
    {
        if (account == null)
        {
            throw new ValidationException("Please supply a valid Account");
        }

        // User must exist before creating an account for it and they must also be active
        var user = userRepository.GetById(account.UserId);
        if (user is null || !user.IsActive)
        {
            throw new ValidationException("User does not exist");
        }
        
        return accountRepository.Insert(account.ToPoco());
    }

    public void UpdateAccountActive(Guid accountId, bool active)
    {
        var account = accountRepository.GetById(accountId);
        if (account == null)
        {
            throw new ArgumentException("Account to be updated does not exist");
        }

        var accountDomain = account.ToDomain();
        accountDomain.UpdateIsActive(active);
        
        accountRepository.Update(accountDomain.ToPoco());
    }
}