using System.ComponentModel.DataAnnotations;
using ApplicationServices.Interfaces;
using ApplicationServices.Mappers;
using Data.POCOs;
using Data.Repositories;
using Domain;

namespace ApplicationServices.Implementations;

public class UserService(IBaseRepository<UserPoco> usersRepository) : IUserService
{
    public IEnumerable<User> GetUsers()
    {
        var usersPoco = usersRepository.Get();

        return usersPoco.ToDomain();
    }

    public User GetUser(Guid userId)
    {
        var userPoco = usersRepository.GetById(userId);

        if (userPoco == null)
        {
            throw new ArgumentException($"User with id {userId} not found");
        }

        return userPoco.ToDomain();
    }

    public Guid AddUser(User user)
    {
        // FiscalNumber would be unique by user, as well as the Id.
        // but since the Id is generated for a new Id, whenever a new User is added, I would assume the fiscalNumber uniqueness
        // as a business rule.
        var existingUser = usersRepository.GetByFilter("FiscalNumber", user.FiscalNumber);

        if (existingUser != null)
        {
            throw new ValidationException("User with the same FiscalNumber already exists");
        }
        
        return usersRepository.Insert(user.ToPoco());
    }

    public void UpdateUserActive(Guid userId, bool active)
    {
        var user = usersRepository.GetById(userId);
        if (user == null)
        {
            throw new ArgumentException("User to be updated does not exist");
        }

        var userDomain = user.ToDomain();
        userDomain.UpdateIsActive(active);
        
        usersRepository.Update(userDomain.ToPoco());
    }
}