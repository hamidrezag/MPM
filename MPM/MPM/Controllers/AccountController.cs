using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MPM.Controllers
{
    public class AccountController : BaseController
    {
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] string phonenumber)
        {
            throw new NotImplementedException();
        }
    }
}
