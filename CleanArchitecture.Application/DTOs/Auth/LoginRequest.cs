using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CleanArchitecture.Application.DTOs.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Identifier is required.")]
        [StringLength(200, ErrorMessage = "Identifier must not exceed 200 characters.")]
        public string Identifier { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(150, ErrorMessage = "Password must not exceed 150 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}
