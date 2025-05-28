using Microsoft.AspNetCore.Identity;
using System;

namespace Lab.Models
{
    public class User : IdentityUser<Guid>
    {
        public string? UserRole { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
