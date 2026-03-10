using Firmaro.Application.Interfaces;
using Firmaro.Domain.Entities;
using Firmaro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Firmaro.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FirmaroDbContext _context;
        
        public UserRepository(FirmaroDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}
