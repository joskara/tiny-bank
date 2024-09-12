using Data.POCOs;
using Domain;

namespace ApplicationServices.Mappers;

public static class AccountMapper
{
    public static IEnumerable<Account> ToDomain(this IEnumerable<AccountPoco> accounts)
    {
        return accounts.Select(o => o.ToDomain());
    }

    public static Account ToDomain(this AccountPoco? poco)
    {
        if (poco == null) return null!;
        
        return new Account(
            poco.Id,
            poco.UserId,
            poco.Balance,
            poco.IsActive,
            poco.CreatedAt,
            poco.ModifiedAt);
    }

    public static AccountPoco ToPoco(this Account account)
    {
        return new AccountPoco
        {
            Id = account.Id,
            UserId = account.UserId,
            Balance = account.Balance,
            IsActive = account.IsActive,
            CreatedAt = account.CreatedAt,
            ModifiedAt = account.ModifiedAt
        };
    }
}