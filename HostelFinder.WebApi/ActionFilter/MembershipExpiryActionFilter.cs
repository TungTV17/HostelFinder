using HostelFinder.Domain.Enums;
using HostelFinder.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HostelFinder.WebApi.ActionFilter
{
    public class MembershipExpiryActionFilter : IAsyncActionFilter
    {
        private readonly HostelFinderDbContext _dbContext;

        public MembershipExpiryActionFilter(HostelFinderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Lấy thông tin userId từ Claims
            var userId = context.HttpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                // Người dùng chưa đăng nhập, tiếp tục xử lý
                await next();
                return;
            }

            // Lấy thông tin người dùng từ database
            var user = await _dbContext.Users.FindAsync(System.Guid.Parse(userId));
            if (user == null)
            {
                // Nếu không tìm thấy người dùng, tiếp tục xử lý
                await next();
                return;
            }

            // Lấy tất cả các gói thành viên của người dùng
            var userMemberships = await _dbContext.UserMemberships
                .Where(um => um.UserId == user.Id)
                .ToListAsync();

            // Kiểm tra nếu tất cả các gói thành viên đều hết hạn
            bool allMembershipsExpired = userMemberships.All(um => um.ExpiryDate < DateTime.Now);

            if (allMembershipsExpired)
            {
                // Nếu tất cả các gói thành viên hết hạn và role là Landlord, từ chối truy cập
                if (user.Role == UserRole.Landlord)
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.HttpContext.Response.WriteAsync("Gói thành viên đã hết hạn!");
                    return;
                }
            }

            // Tiến hành gọi tiếp action method
            var resultContext = await next();

            // You can modify the result if needed here
            if (resultContext.Result is ObjectResult objectResult)
            {
                // Modify the response if needed
                // e.g., wrap the result in a custom response structure
                objectResult.Value = new
                {
                    success = true,
                    data = objectResult.Value
                };
            }
        }
    }
}