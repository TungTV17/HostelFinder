using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Domain.Entities;
using HostelFinder.Domain.Enums;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HostelFinder.Infrastructure.Repositories
{
    public class UserRepository : BaseGenericRepository<User>, IUserRepository
    {
        public UserRepository(HostelFinderDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<bool> CheckEmailExistAsync(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email && !x.IsDeleted);
            if (user == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> CheckPhoneNumberAsync(string phoneNumber)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Phone == phoneNumber && !x.IsDeleted);
            if (user == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> CheckUserNameExistAsync(string userName)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == userName && !x.IsDeleted);
            if (user == null)
            {
                return false;
            }
            return true;
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            var user = await _dbContext.Users.Where(x => x.Email.ToLower().Equals(email.ToLower()) && !x.IsDeleted).FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }
            return user;
        }

        public async Task<User?> FindByUserNameAsync(string userName)
        {
            var user = await _dbContext.Users.Where(x => x.Username.ToLower().Equals(userName.ToLower()) && !x.IsDeleted).FirstOrDefaultAsync();
            if (user == null)
            {
                return null;
            }
            return user;
        }

        public async Task<UserRole> GetRoleAsync(Guid userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId && !x.IsDeleted);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            return user.Role;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbContext.Users
                .Where(u => u.Role != UserRole.Admin)
                .ToListAsync();
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task UpdateAsync(User user)
        {
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User> GetLandlordByHostelIdAsync(Guid hostelId)
        {
            var hostel = await _dbContext.Hostels
                .Include(h => h.Landlord)
                .FirstOrDefaultAsync(h => h.Id == hostelId && !h.IsDeleted);

            return hostel?.Landlord;
        }

        public async Task<Hostel> GetHostelByPostIdAsync(Guid postId)
        {
            var post = await _dbContext.Posts
                .Include(p => p.Hostel)
                .ThenInclude(h => h.Landlord)
                .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);

            return post?.Hostel;
        }

        public Task<User> GetUserByHostelIdAsync(Guid hostelId)
        {
            return _dbContext.Users
                .Include(u => u.Hostels)
                .FirstOrDefaultAsync(u => u.Hostels.Any(h => h.Id == hostelId));
        }

        public async Task<IEnumerable<User>> FilterUsersByActiveStatusAsync(bool isActive)
        {
            return await _dbContext.Users
                .Where(u => u.IsActive == isActive)
                .ToListAsync();
        }

    }
}
