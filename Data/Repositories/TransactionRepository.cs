using Data.POCOs;

namespace Data.Repositories;

public class TransactionRepository : IBaseRepository<TransactionPoco>
{
    private readonly Dictionary<Guid, TransactionPoco> _transactions = new();

    public IEnumerable<TransactionPoco> Get()
    {
        return _transactions.Values;
    }

    public TransactionPoco GetById(Guid id)
    {
        return _transactions.GetValueOrDefault(id)!;
    }

    public void Update(TransactionPoco entity)
    {
        _transactions[entity.Id] = entity;
    }

    public Guid Insert(TransactionPoco entity)
    {
        _transactions.Add(entity.Id, entity);
        return entity.Id;
    }

    public TransactionPoco GetByFilter(string property, string value)
    {
        throw new NotImplementedException();
    }
}