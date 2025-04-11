using Microsoft.AspNetCore.Mvc;

namespace FloreExmaneFinalServiceWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PruebaController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("¡Controlador funcionando!");
        }
    }
}
