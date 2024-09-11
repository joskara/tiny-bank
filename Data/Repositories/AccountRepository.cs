using Data.POCOs;

namespace Data.Repositories;

public class AccountRepository : IBaseRepository<AccountPoco>
{
    private readonly Dictionary<Guid, AccountPoco> _accounts = new();

    public IEnumerable<AccountPoco> Get()
    {
        return _accounts.Values;
    }

    public AccountPoco GetById(Guid id)
    {
        return _accounts[id];
    }

    public void Update(AccountPoco entity)
    {
        _accounts[entity.Id] = entity;
    }

    public Guid Insert(AccountPoco entity)
    {
        _accounts.Add(entity.Id, entity);
        return entity.Id;
    }

    public AccountPoco GetByFilter(string property, string value)
    {
        throw new NotImplementedException();
    }
}