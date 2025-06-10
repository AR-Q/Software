using IAM.Application.AuthenticationService.Interfaces;
using IAM.Contracts.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IAM.Presentation.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserManagerController : ControllerBase
    {
        private readonly IUserUpdateService _userUpdateService;

        public UserManagerController(IUserUpdateService userUpdateService)
        {
            _userUpdateService = userUpdateService;
        }


        [HttpPost("edit")]
        public async Task<ActionResult> Edit([FromBody] UserUpdateVM userUpdate)
        {
            UserUpdateVM user = await _userUpdateService.handle(userUpdate);

            if (user.UserId == -1)
            {
                return BadRequest("sth went wrong");
            }

            return Ok(user);
        }




        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok("amir");
        }

    }
}
