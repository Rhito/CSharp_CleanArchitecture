
namespace CleanArchitecture.Application.DTOs.Order
{
    public class UpdateOrderDto
    {
        public string? OrderCode { get; set; } = string.Empty;
        public DateTime? OrderDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? CustomerId { get; set; }
    }
}
