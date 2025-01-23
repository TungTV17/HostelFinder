using HostelFinder.Application.Common;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Services;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using HostelFinder.Infrastructure.Repositories;
using HostelFinder.Infrastructure.Seeders;
using HostelFinder.Infrastructure.Services;
using HostelFinder.Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HostelFinder.Infrastructure;

public class ServiceRegistration
{
    public static void Configure(IServiceCollection service, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        service.AddDbContext<HostelFinderDbContext>(options =>
            options.UseSqlServer(connectionString));
        service.AddScoped(typeof(IBaseGenericRepository<>), typeof(BaseGenericRepository<>));
        service.AddScoped<IHostelRepository, HostelRepository>();
        service.AddScoped<IUserRepository, UserRepository>();
        service.AddScoped<IPostRepository, PostRepository>();
        service.AddScoped<IAmenityRepository, AmenityRepository>();
        service.AddScoped<IWishlistRepository, WishlistRepository>();
        service.AddScoped<IEmailService, EmailService>();
        service.AddScoped<IAuthAccountService, AuthAccountService>();
        service.AddScoped<IServiceRepository, ServiceRepository>();
        service.AddScoped<IHostelSeeder, HostelSeeder>();
        service.AddScoped<IMembershipRepository, MembershipRepository>();
        service.AddScoped<IS3Service, S3Service>();
        service.AddScoped<IInVoiceRepository, InVoiceRepository>();
        service.AddScoped<IServiceCostRepository, ServiceCostRepository>();
        service.AddScoped<IRoomRepository, RoomRepository>();
        service.AddScoped<IImageRepository, ImageRepository>();
        service.AddScoped<IHostelServiceRepository, HostelServiceRepository>();
        service.AddScoped<IRoomAmentityRepository, RoomAmentityRepository>();
        service.AddScoped<IMeterReadingRepository, MeterReadingRepository>();
        service.AddScoped<IUserMembershipRepository, UserMembershipRepository>();
        service.AddScoped<IAddressRepository, AddressRepository>();
        service.AddScoped<IRoomTenancyRepository, RoomTenancyRepository>();
        service.AddScoped<IVehicleRepository, VehicleRepository>();
        service.AddScoped<ITenantRepository, TenantRepository>();
        service.AddScoped<IRentalContractRepository, RentalContractRepository>();
        service.AddScoped<ITransactionRepository, TransactionRepository>();
        service.AddScoped<IWishlistPostRepository, WishlistPostRepository>();
        service.AddScoped<IMaintenanceRecordRepository, MaintenanceRecordRepository>();
        service.AddScoped<IStoryRepository, StoryRepository>();
        service.AddScoped<IAddressStoryRepository, AddressStoryRepository>();
        service.AddScoped<INotificationRepository, NotificationRepository>();
    }
}