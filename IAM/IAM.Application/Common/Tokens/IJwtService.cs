using IAM.Application.AuthenticationService;
using IAM.Application.AuthenticationService.ViewModels;
using IAM.Contracts.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Application.Common.Tokens
{
    public interface IJwtService
    {
        string Generate(TokenVM user);
        TokenCheckVM GetInfo(string token);
    }
}
