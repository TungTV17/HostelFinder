using DocumentFormat.OpenXml.InkML;
using HostelFinder.Application.DTOs.Story.Requests;
using HostelFinder.Application.DTOs.Story.Responses;
using HostelFinder.Application.Helpers;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;
using HostelFinder.Domain.Enums;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HostelFinder.Infrastructure.Repositories
{
    public class StoryRepository : BaseGenericRepository<Story>, IStoryRepository
    {
        public StoryRepository(HostelFinderDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Story> GetStoryByIdAsync(Guid id)
        {
            return await _dbContext.Stories
                .AsNoTracking()
                .Include(s => s.Images)  
                .Include(s => s.AddressStory)  
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<PagedResponse<List<Story>>> GetAllStoriesAsync(int pageIndex, int pageSize, StoryFilterDto filter)
        {
            var query = _dbContext.Stories
                .AsNoTracking()
                .Where(s => s.BookingStatus == BookingStatus.Accepted && !s.IsDeleted)
                .Include(s => s.Images)
                .Include(s => s.AddressStory)
                .Include(s => s.User)
                .OrderByDescending(s => s.CreatedOn)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Province))
            {
                query = query.Where(s => s.AddressStory.Province == filter.Province);
            }

            if (!string.IsNullOrEmpty(filter.District))
            {
                query = query.Where(s => s.AddressStory.District == filter.District);
            }

            // Lọc theo MinSize nếu có
            if (filter.MinSize.HasValue)
            {
                query = query.Where(s => s.Size >= filter.MinSize.Value);
            }

            // Lọc theo MaxSize nếu có
            if (filter.MaxSize.HasValue)
            {
                query = query.Where(s => s.Size <= filter.MaxSize.Value);
            }

            // Lọc theo MinPrice nếu có
            if (filter.MinRentCost.HasValue)
            {
                query = query.Where(s => s.MonthlyRentCost >= filter.MinRentCost.Value);
            }

            // Lọc theo MaxPrice nếu có
            if (filter.MaxRentCost.HasValue)
            {
                query = query.Where(s => s.MonthlyRentCost <= filter.MaxRentCost.Value);
            }

            var totalRecords = await query.CountAsync();

            var stories = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return PaginationHelper.CreatePagedResponse(stories, pageIndex, pageSize, totalRecords);
        }

        public async Task<PagedResponse<List<Story>>> GetAllStoriesNoCondition(int pageIndex, int pageSize)
        {
            var query = _dbContext.Stories
                .AsNoTracking()
                .Where(s => s.BookingStatus == BookingStatus.Pending && !s.IsDeleted)
                .Include(s => s.Images)
                .Include(s => s.AddressStory)
                .Include(s => s.User)
                .OrderByDescending(s => s.CreatedOn) 
                .AsQueryable();

            var totalRecords = await query.CountAsync();

            var stories = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return PaginationHelper.CreatePagedResponse(stories, pageIndex, pageSize, totalRecords);
        }


        public async Task<PagedResponse<List<Story>>> GetStoriesByUserId(Guid userId, int pageIndex, int pageSize)
        {
            var query = _dbContext.Stories
                .AsNoTracking()
                .Where(s => s.UserId == userId && !s.IsDeleted)
                .Include(s => s.Images)
                .Include(s => s.AddressStory)
                .Include(s => s.User)
                .OrderByDescending(s => s.CreatedOn);

            var totalRecords = await query.CountAsync();

            var stories = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return PaginationHelper.CreatePagedResponse(stories, pageIndex, pageSize, totalRecords);
        }


        public async Task<int> CountStoriesByUserTodayAsync(Guid userId)
        {
            var todayStart = DateTime.Today;  // 00:00:00
            var todayEnd = DateTime.Today.AddDays(1).AddTicks(-1);  // 23:59:59

            return await _dbContext.Stories
                .Where(story => story.CreatedBy == userId.ToString() &&
                                story.CreatedOn >= todayStart &&
                                story.CreatedOn <= todayEnd &&
                                story.BookingStatus == BookingStatus.Accepted)
                .CountAsync();
        }
    }
}
