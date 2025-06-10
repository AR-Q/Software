using IAM.Application.AuthenticationService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Application.AuthenticationService.Interfaces
{
    public interface IUserVerifyService
    {
        Task<UserAuthenticationResult> Handle(string phone, string code);
    }
}
