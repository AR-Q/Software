using IAM.Application.AuthenticationService.Repositories;
using IAM.Application.AuthenticationService.ViewModels;
using IAM.Application.Common.Hash;
using IAM.Application.common.Repositores;
using IAM.Application.Common.Tokens;
using IAM.Contracts.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAM.Domain;

namespace IAM.Application.AuthenticationService
{
    public class AdminLoginService : IAdminLoginService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IHasher _hasher;
        private readonly IJwtService _jwtService;
        public AdminLoginService(IAdminRepository adminRepository,IHasher hasher,IJwtService jwtService)
        {
            _adminRepository = adminRepository;
            _hasher = hasher;
            _jwtService = jwtService;
        }

        public async Task<AdminAuthenticationResult> Handle(LoginVM user)
        {
            Admin admin = await _adminRepository.Find(user.Email, _hasher.Hash(user.Password));

            if (admin == null)
            {
                return null;
            }

            string token = _jwtService.Generate(new TokenVM() { ID = admin.AdminId, Email = admin.Email, Role = "Admin" });

            return new AdminAuthenticationResult() { Admin = admin, Token = token };
        }
    }
}
