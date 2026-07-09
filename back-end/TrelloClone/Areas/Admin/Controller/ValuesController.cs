using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrelloClone.Utilities;

namespace TrelloClone.Areas.Admin.Controller
{
    [Route("[area]/[controller]")]
    [ApiController]
    [Area(SD.ADMIN_AREA)]
    [Authorize]
    public class ValuesController : ControllerBase
    {
    }
}
