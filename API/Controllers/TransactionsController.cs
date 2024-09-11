using API.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1")]
public class TransactionsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<TransactionDto>> Get()
    {
        return new OkResult();
    }
    
    [HttpGet("{transactionId:guid}")]
    public ActionResult<TransactionDto> GetById([FromRoute]Guid transactionId)
    {
        return new OkResult();
    }
    
    [HttpPost]
    public ActionResult Add(TransactionDto transaction)
    {
        return new OkResult();
    }
}