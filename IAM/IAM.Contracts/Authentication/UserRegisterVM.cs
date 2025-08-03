using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Contracts.Authentication
{
    public class UserRegisterVM
    {
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
