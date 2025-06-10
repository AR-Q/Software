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
    public class AdminRepository : IAdminRepository
    {
        private readonly SQLServerDataContext _context;

        public AdminRepository(SQLServerDataContext sQLServerDataContext)
        {
            _context = sQLServerDataContext;
        }

        public async Task<bool> Any(string email)
        {
            return await _context.Admins.AnyAsync(x => x.Email == email);
        }

        public async Task<Admin> Find(string email, string password)
        {
            return await _context.Admins.SingleOrDefaultAsync(x => x.Email == email && x.Password == password);
        }

        public async Task<Admin> Find(int id)
        {
            return await _context.Admins.SingleOrDefaultAsync(x => x.AdminId == id);
        }

        public IEnumerable<Admin> Get()
        {
            return _context.Admins;
        }
    }
}
