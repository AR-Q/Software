using IAM.Application.AuthenticationService;
using IAM.Application.AuthenticationService.Interfaces;
using IAM.Application.AuthenticationService.Repositories;
using IAM.Application.AuthenticationService.ViewModels;
using IAM.Application.Common.Tokens;
using IAM.Contracts.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IAM.Presentation.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserLoginService _userLoginService;
        private readonly IAdminLoginService _adminLoginService;
        private readonly IUserRegisterService _userRegisterService;
        private readonly IUserVerifyService _userVerifyService;
        private readonly ITokenCheck _tokenCheck;
        private readonly IJwtService _jwtService;
        public AuthenticationController(IUserLoginService userLoginService,IAdminLoginService adminLoginService, 
                                        IUserRegisterService userRegisterService, IUserVerifyService userVerifyService,
                                        ITokenCheck tokenCheck, IJwtService jwtService)
        {
            _userLoginService = userLoginService;
            _adminLoginService = adminLoginService;
            _userRegisterService = userRegisterService;
            _userVerifyService = userVerifyService;
            _tokenCheck = tokenCheck;
            _jwtService = jwtService;
        }

        #region Login


        [HttpPost("user")]
        public async Task<ActionResult> LoginUser([FromBody]LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            UserAuthenticationResult result = await _userLoginService.Handle(loginVM);

            if (result == null)
            {
                return BadRequest("User not found");
            }

            return Ok(result);
        }



        [HttpPost("admin")]
        public async Task<ActionResult> LoginAdmin([FromBody] LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            AdminAuthenticationResult result = await _adminLoginService.Handle(loginVM);

            if (result == null)
            {
                return BadRequest("Admin not found");
            }

            return Ok(result);
        }



        #endregion

        #region Register

        [HttpPost("register/user")]
        public async Task<ActionResult> RegisterUser(UserRegisterVM userRegister)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            UserAuthenticationResult result = await _userRegisterService.Handle(userRegister);

            if (result.Token == "email")
            {
                return BadRequest("Phone number is Registered");
            }
            else if (result.Token == "db")
            {
                return BadRequest("Register Failed");
            }


            return Ok(result);
        }


        #endregion


        #region Verify

        [HttpPost("verify/user")]
        public async Task<ActionResult> VerifyUser(string token, string code)
        {
            var res = _jwtService.GetInfo(token);


            UserAuthenticationResult result = await _userVerifyService.Handle(res.Email, code);

            if (result.Token == "error")
            {
                return BadRequest("sth went wrong");
            }
            else if (result.Token == "user")
            {
                return BadRequest("User Not Found");
            }

            return Ok(result);
        }

        #endregion

        [HttpPost("checkToken")]
        public async Task<ActionResult> CheckToken([FromBody] TokenRequest request)
        {
            var res = await _tokenCheck.hanle(request.Token);
            if (res != null)
            {
                return Ok(res);
            }
            return Unauthorized();
        }
    }
}
