using IAM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Application.AuthenticationService.ViewModels
{
    public class TokenCheckVM
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public User User { get; set; }
    }
}
