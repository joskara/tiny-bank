using Data.POCOs;
using Domain;

namespace ApplicationServices.Mappers;

public static class UserMapper
{
    public static IEnumerable<User> ToDomain(this IEnumerable<UserPoco> listOfUsers)
    {
        return listOfUsers.Select(o => o.ToDomain());
    }

    public static User ToDomain(this UserPoco? user)
    {
        if(user is null) return null!;
        
        return new User(
            user.Id,
            user.FirstName,
            user.LastName,
            user.FiscalNumber,
            user.IsActive,
            user.CreatedAt,
            user.ModifiedAt
        );
    }

    public static UserPoco ToPoco(this User user)
    {
        return new UserPoco
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FiscalNumber = user.FiscalNumber,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            ModifiedAt = user.ModifiedAt
        };
    }
}