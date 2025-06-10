using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Contracts.Authentication
{
    public class LoginVM
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
