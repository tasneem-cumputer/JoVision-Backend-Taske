using Microsoft.AspNetCore.Mvc;

namespace JoVision_BackendTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GreeterController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get(string? name)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "anonymous";
            }

            return Content("Hello " + name);
        }
    }
}