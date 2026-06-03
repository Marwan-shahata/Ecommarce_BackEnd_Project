using ECommerce.Common;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    
    protected string GetCurrentUserId()
        => ClaimsHelper.GetUserId(User);

   
    protected IActionResult HandleResult<T>(GeneralResult<T> result)
        => result.Success switch
        {
            true => result switch
            {
                var r when r.Data is null => Ok(result),              
                _ => Ok(result)
            },

            false => HandleError(result)
        };

    private IActionResult HandleError<T>(GeneralResult<T> result)
    {
        return result.Message switch
        {
            "Resource not found" => NotFound(result),
            "Operation Failed" => BadRequest(result),
            _ => BadRequest(result)
        };
    }

    
    protected IActionResult HandleResult(GeneralResult result)
        => result.Success
            ? Ok(result)
            : HandleError(result);

    private IActionResult HandleError(GeneralResult result)
    {
        return result.Message switch
        {
            "Resource not found" => NotFound(result),
            _ => BadRequest(result)
        };
    }
}
