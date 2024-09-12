using Data.POCOs;

namespace Data.Repositories;

public class UserRepository : IBaseRepository<UserPoco>
{
    private readonly Dictionary<Guid, UserPoco> _users = new();

    public IEnumerable<UserPoco> Get()
    {
        return _users.Values;
    }

    public UserPoco GetById(Guid id)
    {
        return _users.GetValueOrDefault(id)!;
    }

    public void Update(UserPoco entity)
    {
        _users[entity.Id] = entity;
    }

    public Guid Insert(UserPoco entity)
    {
        _users.Add(entity.Id, entity);
        return entity.Id;
    }

    // Created a method for filtering properties using Reflection, which is not at all,
    // the ideal situation here, but to simulate a filtering through a database, this was the approach I took.
    public UserPoco GetByFilter(string property, string value)
    {
        return (_users.Values.Any(o => o.GetType().GetProperty(property)!.GetValue(o, null)!.Equals(value)) ? 
            _users.Values.SingleOrDefault(o => o.GetType().GetProperty(property)!.GetValue(o, null)!.Equals(value)) : 
            null)!;
    }
}