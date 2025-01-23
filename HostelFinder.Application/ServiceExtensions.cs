using FluentValidation;
using HostelFinder.Application.DTOs.Users.Requests;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Mappings;
using HostelFinder.Application.Services;
using HostelFinder.Application.Validations.Users;
using HostelFinder.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RoomFinder.Domain.Common.Settings;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace HostelFinder.Application
{
    public class ServiceExtensions
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //register service
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthAccountService, AuthAccountService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IHostelService, HostelService>();
            services.AddScoped<IAmenityService, AmenityService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IMembershipService, MembershipService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IServiceCostService, ServiceCostService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IMeterReadingService, MeterReadingService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IRoomTenancyService, RoomTenancyService>();
            services.AddScoped<IRentalContractService, RentalContractService>();
            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<PasswordHasher<User>>();
            services.AddScoped<IRevenueReportService, RevenueReportService>();
            services.AddScoped<IUserMembershipService, UserMembershipService>();
            services.AddScoped<IOpenAiService, OpenAiService>();
            services.AddScoped<IMaintenanceRecordService, MaintenanceRecordService>();
            services.AddScoped<IStoryService, StoryService>();
            services.AddScoped<INotificationService, NotificationService>();

            //register validation 
            services.AddScoped<IValidator<CreateUserRequestDto>, CreteUserRequestValidation>();

            //register automapper
            services.AddAutoMapper(typeof(GeneralProfile).Assembly);

            //register jwt token
            var jwtSettings = services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));
            services.AddSingleton(jwtSettings);
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = configuration["JWTSettings:Issuer"],
                        ValidAudience = configuration["JWTSettings:Audience"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                    };
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                })
                .AddGoogle(options =>
                {
                    options.ClientId = configuration["Google:ClientId"];
                    options.ClientSecret = configuration["Google:ClientSecret"];
                })
                ;

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("User", policy => policy.RequireRole("User"));
                options.AddPolicy("Landlord", policy => policy.RequireRole("Landlord"));
            });
        }
    }
}