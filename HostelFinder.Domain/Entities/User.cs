using RoomFinder.Domain.Common;
using System.ComponentModel.DataAnnotations;
using HostelFinder.Domain.Common;
using HostelFinder.Domain.Enums;

namespace HostelFinder.Domain.Entities
{
    public class User : BaseEntity
    {
        [MaxLength(100)]
        public string? Username { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }
        
        [MaxLength(100)]
        public string? Password { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(255)]
        public string? AvatarUrl { get; set; } =
            "https://hostel-finder-images.s3.ap-southeast-1.amazonaws.com/Default-Avatar.png";

        [Required]
        public UserRole Role { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        [MaxLength(255)]
        public bool? IsEmailConfirmed { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal WalletBalance { get; set; } = 0;
        
        public string? QRCode { get; set; }
        
        public string? AccountNumber { get; set; }
        
        public string? BankName { get; set; }

        public virtual ICollection<Hostel>? Hostels { get; set; }
        public virtual ICollection<Transaction>? Transactions { get; set; }
        public virtual Wishlist? Wishlists { get; set; }
        public virtual ICollection<UserMembership> UserMemberships { get; set; }
        public virtual ICollection<Story> Stories { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}