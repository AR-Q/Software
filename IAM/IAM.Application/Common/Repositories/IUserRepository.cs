using IAM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Application.common.Repositores
{
    public interface IUserRepository
    {
        IEnumerable<User> Get();
        Task<User> Find(string email, string password);
        Task<User> Find(int id);
        Task<User> Find(string email);
        Task<User> Create(User user);
        Task<User> Update(User user);
        Task<User> Delete(int id);
        Task<User> Delete(User user);
        Task<bool> Any(string email);
    }
}
