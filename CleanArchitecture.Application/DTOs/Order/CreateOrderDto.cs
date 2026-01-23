using CleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CleanArchitecture.Application.DTOs.Order
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage ="Order code is required.")]
        [StringLength(100, ErrorMessage = "Order code must not be exceed 100 characters.")]
        public string OrderCode { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        [Required(ErrorMessage = "Total amount is required.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage ="Total amount must be larger than 0")]
        public decimal TotalAmount { get; set; }
        [Required(ErrorMessage ="Customer Id is required")]
        [Range(1, int.MaxValue, ErrorMessage ="Customer Id is not valid")]
        public int CustomerId { get; set; }
    }
}
