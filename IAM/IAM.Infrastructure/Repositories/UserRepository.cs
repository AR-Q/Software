using IAM.Application.common.Repositores;
using IAM.Domain;
using IAM.Infrastructure.context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SQLServerDataContext _context;

        public UserRepository(SQLServerDataContext sQLServerDataContext)
        {
            _context = sQLServerDataContext;
        }

        public async Task<bool> Any(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }

        public async Task<User> Create(User user)
        {
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> Delete(int id)
        {
            return await Delete(await Find(id));
        }

        public async Task<User> Delete(User user)
        {
            _context.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> Find(string email, string password)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.Email == email && x.Password == password);
        }

        public async Task<User> Find(int id)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.UserId == id);
        }

        public async Task<User> Find(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.Email == email);
        }

        public IEnumerable<User> Get()
        {
            return _context.Users;
        }

        public async Task<User> Update(User user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
