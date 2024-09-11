using Domain;

namespace ApplicationServices.Interfaces;

public interface IUserService
{
    public IEnumerable<User> GetUsers();
    
    public User? GetUser(Guid id);
    
    public Guid AddUser(User user);
    
    public void UpdateUserActive(Guid userId, bool active);
}