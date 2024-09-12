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
public class TransactionsController(ITransactionService transactionService) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<TransactionDto>> Get()
    {
        var transactions = transactionService.GetTransactions();
        return Ok(transactions.ToDto());
    }
    
    [HttpGet("{transactionId:guid}")]
    public ActionResult<TransactionDto> GetById([FromRoute]Guid transactionId)
    {
        var transaction = transactionService.GetTransaction(transactionId);
        if (transaction == null)
        {
            return NotFound();
        }

        return this.Ok(transaction.ToDto());
    }
    
    [HttpPost]
    public ActionResult Add(TransactionDto transaction)
    {
        try
        {
            var transactionId = transactionService.AddTransaction(transaction.ToDomain(default));

            var response = new CreatedResult
            {
                Location = $"{Request.Path}/{transactionId}"
            };
            return response;
        }
        catch (ValidationException validationException)
        {
            return BadRequest(validationException.Message);
        }
    }
}