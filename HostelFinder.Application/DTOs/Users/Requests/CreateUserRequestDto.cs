using System.ComponentModel.DataAnnotations;

namespace HostelFinder.Application.DTOs.Users.Requests
{
    public class CreateUserRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string Username { get; set; }
        
        [Required]
        public string FullName { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }
        [Phone]
        [Required]
        [MaxLength(20)]
        public string Phone { get; set; }
    }
}
