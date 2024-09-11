using System.ComponentModel.DataAnnotations;
using API.Mappers;
using API.Models;
using ApplicationServices.Interfaces;
using Asp.Versioning;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1")]
public class UsersController(IUserService userService) : ControllerBase
{
    /// <summary>
    /// Gets all the available users. To simplify implementation, no pagination will be done, which should be a concern for
    /// an ever-growing list.
    /// </summary>
    /// <returns>List of users</returns>
    ///  <response code="200">Returns all the existing ussers.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response> Not implemented for simplification
    /// <response code="403">Insufficient account permissions.</response> Not implemented for simplification
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> Get()
    {
        var users = userService.GetUsers();
        return this.Ok(users.ToDto());
    }
    
    /// <summary>
    /// Gets an user by Id.
    /// </summary>
    /// <returns>User associated to the identifier</returns>
    ///  <response code="200">Returns the user.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response> Not implemented for simplification
    /// <response code="403">Insufficient account permissions.</response> Not implemented for simplification
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("{userId:guid}")]
    public ActionResult<UserDto> GetById([FromRoute]Guid userId)
    {
        try
        {
            var user = userService.GetUser(userId);
        
            return this.Ok(user!.ToDto());
        }
        catch (ArgumentException argumentException)
        {
            return NotFound(argumentException.Message);
        }
    }
    
    /// <summary>
    /// Adds a new user.
    /// </summary>
    /// <returns>Location header with the new user</returns>
    ///  <response code="201">Returns the location for the new user.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response> Not implemented for simplification
    /// <response code="403">Insufficient account permissions.</response> Not implemented for simplification
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    public ActionResult Add(UserDto user)
    {
        try
        {
            var userId = userService.AddUser(user.ToDomain(default));

            var response = new CreatedResult
            {
                Location = $"{Request.Path}/{userId}"
            };
            return response;
        }
        catch (ValidationException validationException)
        {
            return BadRequest(validationException.Message);
        }
    }
    
    /// <summary>
    /// Update the active property in a user.
    /// </summary>
    /// <returns>Status response</returns>
    /// <response code="204">No Content.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response> Not implemented for simplification
    /// <response code="403">Insufficient account permissions.</response> Not implemented for simplification
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPut("{userId:guid}/active")]
    public ActionResult Update([FromBody] bool isActive, [FromRoute] Guid userId)
    {
        try
        {
            userService.UpdateUserActive(userId, isActive);
            return new NoContentResult();
        }
        catch (ArgumentException argumentException)
        {
            return NotFound(argumentException.Message);
        }
    }
}