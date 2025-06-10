using IAM.Application.AuthenticationService.Interfaces;
using IAM.Application.AuthenticationService.ViewModels;
using IAM.Application.common.Repositores;
using IAM.Application.Common.Repositories;
using IAM.Application.Common.Tokens;
using IAM.Contracts.Authentication;
using IAM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Application.AuthenticationService
{
    public class UserVerifyService : IUserVerifyService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly ICachingContext _cachingContext;

        public UserVerifyService(IUserRepository userRepository, IJwtService jwtService, ICachingContext cachingContext)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _cachingContext = cachingContext;
        }

        public async Task<UserAuthenticationResult> Handle(string phone, string code)
        {
            User user = await _userRepository.Find(phone);

            if (user == null)
            {
                return new UserAuthenticationResult() { User = user, Token = "user" };
            }

            string res = _cachingContext.Get(phone);

            if (res == code)
            {
                user.IsActive = true;
                user = await _userRepository.Update(user);
                string token = _jwtService.Generate(new TokenVM() { ID = user.UserId, Email = user.Email,Role = "User"});
                return new UserAuthenticationResult() { User = user, Token = token };
            }

            return new UserAuthenticationResult() { User = user, Token = "error" };
        }
    }
}
