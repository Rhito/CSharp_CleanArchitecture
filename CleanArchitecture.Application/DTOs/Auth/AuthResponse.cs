using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.DTOs.Auth
{
    public class AuthResponse
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string JwToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
     
        public bool IsVerified { get; set; }

        public List<string> Roles { get; set; } = [];
    }
}
