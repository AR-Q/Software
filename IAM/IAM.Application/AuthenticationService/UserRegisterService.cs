using IAM.Application.AuthenticationService.Interfaces;
using IAM.Application.AuthenticationService.ViewModels;
using IAM.Application.common.Repositores;
using IAM.Application.Common.Code;
using IAM.Application.Common.Hash;
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
    public class UserRegisterService : IUserRegisterService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IHasher _hasher;
        private readonly IJwtService _jwtService;
        private readonly ICodeGenerator _codeGenerator;
        private readonly ICachingContext _caching;
        private readonly IEmailService _emailService;

        public UserRegisterService(IUserRepository userRepository, IAdminRepository adminRepository,
                                   IHasher hasher, IJwtService jwtService, ICodeGenerator codeGenerator
                                   ,ICachingContext cachingContext, IEmailService emailService)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _hasher = hasher;
            _jwtService = jwtService;
            _codeGenerator = codeGenerator;
            _caching = cachingContext;
            _emailService = emailService;   
        }

        public async Task<UserAuthenticationResult> Handle(UserRegisterVM userRegister)
        {
            // some more checks like national Id
            if (await _adminRepository.Any(userRegister.Email) || await _userRepository.Any(userRegister.Email))
            {
                return new UserAuthenticationResult() { User = null, Token = "phone"};
            }

            User user = new User()
            {
                Email = userRegister.Email,
                FirstName = userRegister.FirstName,
                LastName = userRegister.LastName,
                MediaId = 0,
                Password = _hasher.Hash(userRegister.Password),
                RegisterDate = DateTime.Now,
                Wallet = 0,
                IsActive = false
            };

            try
            {
                user = await _userRepository.Create(user);
            }
            catch
            {
                return new UserAuthenticationResult() { User = null, Token = "db" }; ;
            }

            string token = _jwtService.Generate(new TokenVM() { ID = user.UserId, Email = user.Email, Role = "User" });

            string code = _codeGenerator.GenerateCode();

            // Redis for Saving OTP
            _caching.Set(user.Email, code);
            await _emailService.Send(new EmailVM() { ToEmail = user.Email ,Subject="Register Code",Message=code});

            

            return new UserAuthenticationResult() { User = user,Token = token};
        }
    }
}
