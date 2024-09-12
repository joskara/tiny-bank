using System;
using System.Collections.Generic;
using System.Linq;
using API.Models;
using Domain;

namespace API.Mappers;

public static class UserMapper
{
    public static IEnumerable<UserDto> ToDto(this IEnumerable<User> users)
    {
        return users.Select(o => o.ToDto());
    }

    public static UserDto ToDto(this User user)
    {
        if(user == null!) return null!;
        
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FiscalNumber = user.FiscalNumber,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            ModifiedAt = user.ModifiedAt,
        };
    }

    public static User ToDomain(this UserDto userDto, Guid id)
    {
        return new User(
            id,
            userDto.FirstName,
            userDto.LastName,
            userDto.FiscalNumber,
            userDto.IsActive,
            userDto.CreatedAt,
            userDto.ModifiedAt
        );
    }
}