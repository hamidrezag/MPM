using Application.AccountServices;
using Application.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MPM.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountServices _accountServices;

        public AccountController(IAccountServices accountServices)
        {
            _accountServices = accountServices;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] GetTokenReqDto req)
        {
            var res = await _accountServices.GetToken(req.Phonenumber);
            return Ok(new GetTokenResDto
            {
                Token = res
            }) ;
        }
    }
}
