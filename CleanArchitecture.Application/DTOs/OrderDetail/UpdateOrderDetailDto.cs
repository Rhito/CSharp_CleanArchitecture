using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CleanArchitecture.Application.DTOs.OrderDetail
{
    public class UpdateOrderDetailDto
    {
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int? Quantity { get; set; }
        [Required(ErrorMessage = "UnitPrice is required.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "UnitPrice must be a positive value.")]
        public decimal? UnitPrice { get; set; }
    }
}
