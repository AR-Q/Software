using IAM.Application.AuthenticationService.Interfaces;
using IAM.Application.AuthenticationService.ViewModels;
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
        public TokenCheck(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public TokenCheckVM hanle(string token)
        {
            var res = _jwtService.GetInfo(token);
            return res;

        }
    }
}
