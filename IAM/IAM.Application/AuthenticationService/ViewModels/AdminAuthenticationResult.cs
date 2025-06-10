using IAM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Application.AuthenticationService.ViewModels
{
    public class AdminAuthenticationResult
    {
        public Admin Admin { get; set; }
        public string Token { get; set; }
    }
}
