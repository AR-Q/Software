using IAM.Application.AuthenticationService.ViewModels;
using IAM.Contracts.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Application.AuthenticationService.Interfaces
{
    public interface IUserRegisterService
    {
        Task<UserAuthenticationResult> Handle(UserRegisterVM userRegister);
    }
}
