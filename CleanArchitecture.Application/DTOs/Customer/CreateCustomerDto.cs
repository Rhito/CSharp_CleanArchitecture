using CleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CleanArchitecture.Application.DTOs.Customer
{
    public class CreateCustomerDto
    {
        // 1. Name: Required, max length 100
        [Required(ErrorMessage = "Customer name is required.")]
        [StringLength(100, ErrorMessage = "Customer name must not exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        // 2. Email: Required, must be a valid email format
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(150, ErrorMessage = "Email address must not exceed 150 characters.")]
        public string Email { get; set; } = string.Empty;

        // 3. Phone Number: Optional, but must be valid if provided
        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(15, ErrorMessage = "Phone number must not exceed 15 digits.")]
        public string? PhoneNumber { get; set; }

        // 4. Address: Optional, max length to prevent database spam
        [StringLength(255, ErrorMessage = "Address must not exceed 255 characters.")]
        public string? Address { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
