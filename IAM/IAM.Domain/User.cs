using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Domain
{
    public class User
    {
        public int UserId { get; set; }
        public string Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? NationalID { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public int Wallet { get; set; }


        public bool IsActive { get; set; }

        // relations
        public int MediaId { get; set; }
    }
}
