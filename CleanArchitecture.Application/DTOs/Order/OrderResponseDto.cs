using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.DTOs.Order
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int CustomerId { get; set; }
    }
}
