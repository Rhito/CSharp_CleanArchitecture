using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.DTOs.OrderDetail
{
    public class OrderDetailResponseDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
