using System.ComponentModel.DataAnnotations;

namespace HostelFinder.Application.DTOs.Auth.Requests
{
    public class AuthenticationRequest
    {
        [MaxLength(50)]
        public string UserName { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [MaxLength(50)]
        public string Password { get; set; }

    }
}
