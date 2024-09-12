using System;
using System.Collections.Generic;
using System.Linq;
using API.Models;
using Domain;

namespace API.Mappers;

public static class AccountMapper
{
    public static IEnumerable<AccountDto> ToDto(this IEnumerable<Account> accounts)
    {
        return accounts.Select(o => o.ToDto());
    }

    public static AccountDto ToDto(this Account account)
    {
        if(account == null!) return null!;
        
        return new AccountDto
        {
            Id = account.Id,
            UserId = account.UserId,
            Balance = account.Balance,
            IsActive = account.IsActive,
            CreatedAt = account.CreatedAt,
            ModifiedAt = account.ModifiedAt,
        };
    }

    public static Account ToDomain(this AccountDto dto, Guid id)
    {
        return new Account(
            id,
            dto.UserId,
            dto.Balance,
            dto.IsActive,
            dto.CreatedAt,
            dto.ModifiedAt
        );
    }
}