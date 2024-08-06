using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Entities;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("Register")]
        public async Task<ActionResult> RegisterAsync(RegisterModel model)
        {
            var resault = await _authService.RegesterAsync(model);

            if(resault.IsAuthenticated is false)
                return BadRequest(resault.Message);

            return Ok(resault);
        }

        [HttpPost("GetToken")]
        public async Task<ActionResult> GetTokenAsync(TokenRequestModel model)
        {
            var resault = await _authService.GetokenAsync(model);

            if (resault.IsAuthenticated is false)
                return BadRequest(resault.Message);

            return Ok(resault);
        }


        [HttpPost("AddRole")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddRoleAsync(AddRoleModel RoleModel)
        {
            var resault = await _authService.AddRoleAsync(RoleModel);

            if(!string.IsNullOrEmpty(resault))
                return BadRequest(resault);

            return Ok(RoleModel);
        }
    }
}
