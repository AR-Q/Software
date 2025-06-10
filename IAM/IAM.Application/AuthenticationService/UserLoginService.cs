using IAM.Application.AuthenticationService.Repositories;
using IAM.Application.AuthenticationService.ViewModels;
using IAM.Application.common.Repositores;
using IAM.Application.Common.Hash;
using IAM.Application.Common.Tokens;
using IAM.Contracts.Authentication;
using IAM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Application.AuthenticationService
{
    public class UserLoginService : IUserLoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHasher _hasher;
        private readonly IJwtService _jwtService;
        public UserLoginService(IUserRepository userRepository, IHasher hasher, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _hasher = hasher;
            _jwtService = jwtService;
            _hasher = hasher;
        }

        public async Task<UserAuthenticationResult> Handle(LoginVM user)
        {
            User User = await _userRepository.Find(user.Email,_hasher.Hash(user.Password));

            if (User == null)
            {
                return null;
            }

            string token = _jwtService.Generate(new TokenVM() { ID = User.UserId, Email = User.Email, Role = "User" });

            return new UserAuthenticationResult() { User = User, Token = token };
        }
    }
}
