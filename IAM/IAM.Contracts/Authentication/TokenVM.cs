using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Contracts.Authentication
{
    public class TokenVM
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
