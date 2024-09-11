namespace Data.Repositories;

public interface IBaseRepository<T>
where T : class
{
    IEnumerable<T> Get();

    T GetById(Guid id);
    
    void Update(T entity);

    Guid Insert(T entity);
    
    T GetByFilter(string property, string value);
}