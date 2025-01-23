using HostelFinder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HostelFinder.Infrastructure.Context;

public class HostelFinderDbContext : DbContext
{
    public DbSet<Hostel> Hostels { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<RoomDetails> RoomDetails { get; set; }
    public DbSet<Amenity> Amenities { get; set; }
    public DbSet<RoomAmenities> RoomAmenities { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<ServiceCost> ServiceCosts { get; set; }
    public DbSet<BlackListToken> BlackListTokens { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<WishlistPost> WishlistPosts { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<MembershipServices?> MembershipServices { get; set; }
    public DbSet<UserMembership> UserMemberships { get; set; }
    public DbSet<Invoice> InVoices { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<HostelServices> HostelServices { get; set; }
    public DbSet<MeterReading> MeterReadings { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<RoomTenancy> RoomTenancies { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<RentalContract> RentalContracts { get; set; }
    public DbSet<Story> Stories { get; set; }
    public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }
    public DbSet<AddressStory> AddressStories { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    public HostelFinderDbContext(DbContextOptions<HostelFinderDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                //.AddJsonFile("appsettings.Development.json")
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configurationRoot = builder.Build();
            optionsBuilder.UseSqlServer(configurationRoot.GetConnectionString("DefaultConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Address entity
        modelBuilder.Entity<Address>()
            .HasKey(a => a.Id);
        modelBuilder.Entity<Address>()
            .HasOne(a => a.Hostel)
            .WithOne(h => h.Address)
            .HasForeignKey<Address>(a => a.HostelId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Amenity entity
        modelBuilder.Entity<Amenity>()
            .HasKey(a => a.Id);

        // Configure Hostel entity
        modelBuilder.Entity<Hostel>()
            .HasOne(h => h.Landlord)
            .WithMany(u => u.Hostels)
            .HasForeignKey(h => h.LandlordId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Hostel>()
            .HasMany(h => h.HostelServices)
            .WithOne(s => s.Hostel)
            .HasForeignKey(s => s.HostelId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Hostel>()
            .HasMany(h => h.Posts)
            .WithOne(p => p.Hostel)
            .HasForeignKey(p => p.HostelId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Hostel>()
            .HasMany(h => h.Images)
            .WithOne(i => i.Hostel)
            .HasForeignKey(i => i.HostelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Hostel>()
            .HasMany(h => h.Rooms)
            .WithOne(r => r.Hostel)
            .HasForeignKey(r => r.HostelId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Image entity
        modelBuilder.Entity<Image>()
            .HasKey(i => i.Id);
        modelBuilder.Entity<Image>()
            .HasOne(i => i.Hostel)
            .WithMany(h => h.Images)
            .HasForeignKey(i => i.HostelId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Image>()
            .HasOne(i => i.Post)
            .WithMany(p => p.Images)
            .HasForeignKey(i => i.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Membership entity
        modelBuilder.Entity<Membership>()
            .HasKey(m => m.Id);
        modelBuilder.Entity<Membership>(entity =>
        {
            entity.Property(r => r.Price)
                .HasPrecision(18, 2);
        });

        // Configure MembershipServices entity
        modelBuilder.Entity<MembershipServices>()
        .HasKey(ms => ms.Id);
        modelBuilder.Entity<MembershipServices>()
            .HasMany(ms => ms.Posts)
            .WithOne(p => p.MembershipServices)
            .HasForeignKey(p => p.MembershipServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Post entity
        modelBuilder.Entity<Post>()
            .HasKey(p => p.Id);
        modelBuilder.Entity<Post>()
            .HasOne(p => p.Hostel)
            .WithMany(h => h.Posts)
            .HasForeignKey(p => p.HostelId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Post>()
            .HasOne(p => p.Room)
            .WithMany(r => r.Posts)
            .HasForeignKey(p => p.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Room entity
        modelBuilder.Entity<Room>()
            .HasKey(r => r.Id);
        modelBuilder.Entity<Room>()
            .HasMany(r => r.RoomAmenities)
            .WithOne(ra => ra.Room)
            .HasForeignKey(ra => ra.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Room>()
            .HasMany(r => r.Posts)
            .WithOne(p => p.Room)
            .HasForeignKey(p => p.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Room>()
            .HasOne(r => r.Hostel)
            .WithMany(h => h.Rooms)
            .HasForeignKey(r => r.HostelId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Room>(entity =>
        {
            entity.Property(r => r.Deposit)
                .HasPrecision(18, 2);

            entity.Property(r => r.MonthlyRentCost)
                .HasPrecision(18, 2);
        });
        modelBuilder.Entity<Room>(entity =>
        {
            entity.Property(e => e.Size)
                .HasPrecision(18, 2);
        });

        // Configure RoomAmenities entity
        modelBuilder.Entity<RoomAmenities>()
            .HasKey(ra => new { ra.RoomId, ra.AmenityId });
        modelBuilder.Entity<RoomAmenities>()
            .HasOne(ra => ra.Room)
            .WithMany(r => r.RoomAmenities)
            .HasForeignKey(ra => ra.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<RoomAmenities>()
            .HasOne(ra => ra.Amenity)
            .WithMany(a => a.RoomAmenities)
            .HasForeignKey(ra => ra.AmenityId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure RoomDetails entity
        modelBuilder.Entity<RoomDetails>()
            .HasKey(rd => rd.RoomId);
        modelBuilder.Entity<RoomDetails>()
            .HasOne(rd => rd.Room)
            .WithOne(r => r.RoomDetails)
            .HasForeignKey<RoomDetails>(rd => rd.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Service-Hostel entity
        modelBuilder.Entity<Service>()
            .HasKey(s => s.Id);
        modelBuilder.Entity<Service>()
            .HasMany(s => s.HostelServices)
            .WithOne(h => h.Services)
            .HasForeignKey(s => s.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure ServiceCost entity
        modelBuilder.Entity<ServiceCost>()
            .HasOne(sc => sc.Hostel)
            .WithMany(r => r.ServiceCosts)
            .HasForeignKey(sc => sc.HostelId);

        modelBuilder.Entity<ServiceCost>()
            .HasOne(sc => sc.Service)
            .WithMany(s => s.ServiceCosts)
            .HasForeignKey(sc => sc.ServiceId);

        // Configure User entity
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);
        modelBuilder.Entity<User>()
            .HasMany(u => u.Hostels)
            .WithOne(h => h.Landlord)
            .HasForeignKey(h => h.LandlordId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<User>()
            .HasMany(u => u.UserMemberships)
            .WithOne(um => um.User)
            .HasForeignKey(um => um.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<User>()
            .HasOne(u => u.Wishlists)
            .WithOne(w => w.User)
            .HasForeignKey<Wishlist>(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure UserMembership entity
        modelBuilder.Entity<UserMembership>()
            .HasKey(um => new { um.UserId, um.MembershipId });
        modelBuilder.Entity<UserMembership>()
            .HasOne(um => um.User)
            .WithMany(u => u.UserMemberships)
            .HasForeignKey(um => um.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserMembership>()
            .HasOne(um => um.Membership)
            .WithMany(m => m.UserMemberships)
            .HasForeignKey(um => um.MembershipId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Wishlist entity
        modelBuilder.Entity<Wishlist>()
            .HasKey(w => w.Id);
        modelBuilder.Entity<Wishlist>()
            .HasMany(w => w.WishlistPosts)
            .WithOne(wp => wp.Wishlist)
            .HasForeignKey(wp => wp.WishlistId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure WishlistPost entity
        modelBuilder.Entity<WishlistPost>()
            .HasKey(wp => new { wp.WishlistId, wp.PostId });
        modelBuilder.Entity<WishlistPost>()
            .HasOne(wp => wp.Wishlist)
            .WithMany(w => w.WishlistPosts)
            .HasForeignKey(wp => wp.WishlistId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<WishlistPost>()
            .HasOne(wp => wp.Post)
            .WithMany(p => p.WishlistPosts)
            .HasForeignKey(wp => wp.PostId)
            .OnDelete(DeleteBehavior.Cascade);


        //config Room-Invoice
        modelBuilder.Entity<Invoice>()
         .HasOne(i => i.Room)
         .WithMany(r => r.Invoices)
         .HasForeignKey(i => i.RoomId);

        modelBuilder.Entity<Invoice>()
            .Property(i => i.AmountPaid)
            .HasColumnType("decimal(18, 2)");

        //config invoice - invoiceDetails
        modelBuilder.Entity<InvoiceDetail>()
            .HasOne(id => id.Invoice)
            .WithMany(i => i.InvoiceDetails)
            .HasForeignKey(id => id.InvoiceId);

        modelBuilder.Entity<InvoiceDetail>()
            .HasOne(id => id.Service)
            .WithMany(s => s.InvoiceDetails)
            .HasForeignKey(id => id.ServiceId);

        //config meterReading - Room

        modelBuilder.Entity<MeterReading>()
             .HasOne(m => m.Room)
             .WithMany(r => r.MeterReadings)
             .HasForeignKey(m => m.RoomId)
             .OnDelete(DeleteBehavior.Cascade);

        //config MeterReading - Service
        modelBuilder.Entity<MeterReading>()
         .HasOne(m => m.Service)
         .WithMany(s => s.MeterReadings)
         .HasForeignKey(m => m.ServiceId)
         .OnDelete(DeleteBehavior.Cascade);

        // Configure User and Transactions relationship
        modelBuilder.Entity<User>()
            .HasMany(u => u.Transactions)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Transaction entity
        modelBuilder.Entity<Transaction>()
            .HasKey(t => t.Id);
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2); // For monetary values

        // Configure the relationship between Story and Address
        modelBuilder.Entity<Story>()
            .HasOne(s => s.AddressStory)
            .WithOne(a => a.Story)
            .HasForeignKey<AddressStory>(a => a.StoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the relationship between Story and Image
        modelBuilder.Entity<Story>()
            .HasMany(s => s.Images)
            .WithOne(i => i.Story)
            .HasForeignKey(i => i.StoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the relationship between Story and User
        modelBuilder.Entity<Story>()
            .HasOne(s => s.User)
            .WithMany(u => u.Stories)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
       
        modelBuilder.Entity<AddressStory>()
           .HasOne(a => a.Story)
           .WithOne(s => s.AddressStory)
           .HasForeignKey<AddressStory>(a => a.StoryId)
           .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Story>(entity =>
        {
            entity.Property(e => e.MonthlyRentCost)
                .HasPrecision(18, 2);
        });

        // Cấu hình mối quan hệ giữa User và Notification
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)                
            .WithMany(u => u.Notifications)   
            .HasForeignKey(n => n.UserId)     
            .OnDelete(DeleteBehavior.Cascade);
    }
}