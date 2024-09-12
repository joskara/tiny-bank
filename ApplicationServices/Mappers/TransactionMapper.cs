using Data.POCOs;
using Domain;

namespace ApplicationServices.Mappers;

public static class TransactionMapper
{
    public static IEnumerable<Transaction> ToDomain(this IEnumerable<TransactionPoco> listOfPocos)
    {
        return listOfPocos.Select(o => o.ToDomain());
    }

    public static Transaction ToDomain(this TransactionPoco poco)
    {
        if(poco == null!) return null!;

        return new Transaction(
            poco.Id,
            poco.OriginAccountId,
            poco.DestinationAccountId,
            poco.Amount,
            poco.Type,
            poco.Status,
            poco.DateCreated,
            poco.CreatedBy);
    }

    public static TransactionPoco ToPoco(this Transaction transaction)
    {
        return new TransactionPoco
        {
            Id = transaction.Id,
            OriginAccountId = transaction.OriginAccountId,
            DestinationAccountId = transaction.DestinationAccountId,
            Amount = transaction.Amount,
            Status = transaction.Status,
            Type = transaction.Type,
            DateCreated = transaction.DateCreated,
            CreatedBy = transaction.CreatedBy
        };
    }
}