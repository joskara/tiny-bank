using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using API.Mappers;
using API.Models;
using ApplicationServices.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1")]
public class AccountsController(IAccountService accountService) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<AccountDto>> Get()
    {
        var accounts = accountService.GetAccounts();
        return Ok(accounts.ToDto());
    }
    
    [HttpGet("{accountId:guid}")]
    public ActionResult<AccountDto> GetById([FromRoute]Guid accountId)
    {
        var account = accountService.GetAccount(accountId);
        if (account == null)
        {
            return NotFound();
        }

        return this.Ok(account.ToDto());
    }
    
    [HttpPost]
    public ActionResult Add(AccountDto account)
    {
        try
        {
            var accountId = accountService.AddAccount(account.ToDomain(default));

            var response = new CreatedResult
            {
                Location = $"{Request.Path}/{accountId}"
            };
            return response;
        }
        catch (ValidationException validationException)
        {
            return BadRequest(validationException.Message);
        }
    }
    
    [HttpPut("{accountId:guid}/active")]
    public ActionResult<AccountDto> Modify([FromBody] bool isActive, [FromRoute] Guid accountId)
    {
        try
        {
            accountService.UpdateAccountActive(accountId, isActive);
            return new NoContentResult();
        }
        catch (ArgumentException argumentException)
        {
            return NotFound(argumentException.Message);
        }
    }
}