using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderCode {get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int CustomerId { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = [];
        public virtual Customer Customer { get; set; } = null!;
    }
}
