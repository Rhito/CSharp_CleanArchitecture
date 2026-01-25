using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.DTOs.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
