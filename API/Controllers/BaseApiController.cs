using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Controller]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        
    }
}