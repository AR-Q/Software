using IAM.Contracts.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Application.AuthenticationService.Interfaces
{
    public interface IUserUpdateService
    {
        Task<UserUpdateVM> handle(UserUpdateVM userUpdate);
    }
}
