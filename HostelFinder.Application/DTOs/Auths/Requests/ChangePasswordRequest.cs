using System.ComponentModel.DataAnnotations;

namespace HostelFinder.Application.DTOs.Auth.Requests
{
    public class ChangePasswordRequest
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
