using CleanArchitecture.Application.DTOs.Order;
using CleanArchitecture.Application.Filters;


namespace CleanArchitecture.Application.IService
{
    public interface IOrderService
    {
        Task<OrderResponseDto> GetByIdAsync(int id);
        Task<IEnumerable<OrderResponseDto>> SearchAsync(OrderSearchFilter filter);
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<OrderResponseDto> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto);
        Task<bool> DeleteOrderAsync(int id);
    }
}
