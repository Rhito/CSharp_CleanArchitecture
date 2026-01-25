using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CleanArchitecture.Application.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage ="FullName is required")]
        [StringLength(100, ErrorMessage = "FullName must not exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(150, ErrorMessage = "Email address must not exceed 150 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(150, ErrorMessage = "Password must not exceed 150 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "UserName name is required.")]
        [StringLength(100, ErrorMessage = "UserName name must not exceed 100 characters.")]
        public string UserName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(15, ErrorMessage = "Phone number must not exceed 15 digits.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
