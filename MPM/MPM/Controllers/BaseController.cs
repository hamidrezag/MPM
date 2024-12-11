using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MPM.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("/api/v{version:apiVersion}/[Controller]/[Action]")]
    public class BaseController:ControllerBase
    {
        

    }
}
