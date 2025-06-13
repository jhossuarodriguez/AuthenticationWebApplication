using System;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationWebApplication.Models
{
    public class RevokedToken
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Jti { get; set; } = null!;
        [Required]
        public DateTime RevokedAt { get; set; }
    }
}