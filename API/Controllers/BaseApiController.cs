using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Application.Core;

namespace API.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
       private IMediator _mediator;
       
       protected IMediator Mediator=> _mediator??=HttpContext.RequestServices.GetService<IMediator>();

       protected ActionResult HandleResult<T>(Results<T> result){
        if (result==null) return NotFound();
        if (result.IsSuccess && result.Value !=null) return Ok(result.Value);
        if (result.IsSuccess && result.Value !=null) return NotFound();
        return BadRequest(result.Error);
       }
    }
}