using IAM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Application.common.Repositores
{
    public interface IAdminRepository
    {

        IEnumerable<Admin> Get();
        Task<Admin> Find(string email,string password);
        Task<bool> Any(string email);
        Task<Admin> Find(int id);
    }
}
