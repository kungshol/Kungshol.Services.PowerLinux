using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Kungshol.Services.PowerLinux.Controllers
{
    [ApiController]
    public class StatusController : Controller
    {
        [Route("~/")]
        [HttpGet]
        public async Task<IActionResult> Index([FromServices] UpsStatusService statusService)
        {
            UpsStatus upsStatus = await statusService.GetStatusAsync();

            return new ObjectResult(upsStatus);
        }
    }
}