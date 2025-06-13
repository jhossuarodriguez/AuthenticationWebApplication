using System;
using System.Collections.Generic;

namespace AuthenticationWebApplication.Models;

public partial class User
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public string? PasswordSalt { get; set; }

    public string? PhoneNumber { get; set; }

    public bool? IsEmailConfirmed { get; set; }

    public bool? IsPhoneConfirmed { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsLockedOut { get; set; }

    public int? AccessFailedCount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public bool? TwoFactorEnabled { get; set; }

    public string? SecurityStamp { get; set; }

    public string? Role { get; set; }
}
