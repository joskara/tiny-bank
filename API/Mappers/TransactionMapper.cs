using System;
using System.Collections.Generic;
using System.Linq;
using API.Models;
using Domain;

namespace API.Mappers;

public static class TransactionMapper
{
    public static Transaction ToDomain(this TransactionDto transaction, Guid id)
    {
        if (transaction == null!) return null!;

        return new Transaction(
            id,
            transaction.OriginAccountId,
            transaction.DestinationAccountId,
            transaction.Amount,
            transaction.Type,
            transaction.Status,
            transaction.DateCreated,
            transaction.CreatedBy);
    }

    public static IEnumerable<TransactionDto> ToDto(this IEnumerable<Transaction> transactions)
    {
        return transactions.Select(o => o.ToDto());
    }

    public static TransactionDto ToDto(this Transaction transaction)
    {
        if (transaction == null!) return null!;

        return new TransactionDto
        {
            Id = transaction.Id,
            DestinationAccountId = transaction.DestinationAccountId,
            OriginAccountId = transaction.OriginAccountId,
            Amount = transaction.Amount,
            Status = transaction.Status,
            Type = transaction.Type,
            CreatedBy = transaction.CreatedBy,
            DateCreated = transaction.DateCreated,
        };
    }
}