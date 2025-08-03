using IAM.Application.AuthenticationService.Interfaces;
using IAM.Application.AuthenticationService.ViewModels;
using IAM.Application.common.Repositores;
using IAM.Application.Common.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Application.AuthenticationService
{
    public class TokenCheck : ITokenCheck
    {
        private readonly IJwtService _jwtService;
        private readonly IUserRepository _userRepository;
        public TokenCheck(IJwtService jwtService, IUserRepository userRepository)
        {
            _jwtService = jwtService;
            _userRepository = userRepository;
        }

        public async Task<TokenCheckVM> hanle(string token)
        {
            var res = _jwtService.GetInfo(token);

            if (res.Role == "User")
            {
                res.User = await _userRepository.Find(res.ID);
                res.User.Password = "*";
            }

            return res;

        }
    }
}
