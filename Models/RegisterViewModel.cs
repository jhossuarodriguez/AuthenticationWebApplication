using System.ComponentModel.DataAnnotations;

namespace AuthenticationWebApplication.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(20, MinimumLength = 4)]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}