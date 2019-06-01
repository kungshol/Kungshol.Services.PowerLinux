using System.Threading.Tasks;
using Kungshol.Services.PowerLinux.Routes;
using Kungshol.Services.PowerLinux.Ups;
using Microsoft.AspNetCore.Mvc;

namespace Kungshol.Services.PowerLinux.Controllers
{
    [ApiController]
    public class StatusController : Controller
    {
        [Route(RouteConstants.StatusApiRoute, Name = RouteConstants.StatusApiRouteName)]
        [HttpGet]
        public async Task<ActionResult<UpsStatus>> Status([FromServices] UpsStatusService statusService)
        {
            UpsStatus upsStatus = await statusService.GetStatusAsync();

            return upsStatus;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        [Route(RouteConstants.RootRoute, Name = RouteConstants.RootRouteName)]
        public IActionResult Index()
        {
            return Redirect(RouteConstants.DocsRoute);
        }
    }
}