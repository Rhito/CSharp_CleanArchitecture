using CleanArchitecture.Application.DTOs.Order;
using CleanArchitecture.Application.Filters;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository) {
            _orderRepository = orderRepository;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            if(createOrderDto == null)
                throw new ArgumentNullException(nameof(createOrderDto));

            var order = new Order
            {
                OrderCode = createOrderDto.OrderCode,
                OrderDate = createOrderDto.OrderDate,
                TotalAmount = createOrderDto.TotalAmount,
                CustomerId = createOrderDto.CustomerId
            };

            var response = await _orderRepository.AddAsync(order);
            return MapToResponse(response);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            // kiểm tra tồn tại
            var order = await _orderRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Order with ID {id} not found."); ;
            // xóa order    
            return await _orderRepository.DeleteAsync(id);

        }

        public async Task<OrderResponseDto> GetByIdAsync(int id)
        {
            // kiểm tra tồn tại
            var order = await _orderRepository.GetByIdAsync(id);
            if(order == null) throw new KeyNotFoundException($"Order with ID {id} not found.");
            return MapToResponse(order);
        }

        public async Task<IEnumerable<OrderResponseDto>> SearchAsync(OrderSearchFilter filter)
        {
            if(filter.PageNumber <= 0 || filter.PageSize <= 0)
                throw new ArgumentException("PageNumber and PageSize must be greater than zero.");
            if(string.IsNullOrWhiteSpace(filter.Keyword))
                throw new ArgumentException("Keyword must not be null or empty.");
            var orders = await _orderRepository.SearchAsync(filter);
            return orders.Select(o => MapToResponse(o));
        }

        public async Task<OrderResponseDto> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto)
        {
            if(updateOrderDto == null)
                throw new ArgumentNullException(nameof(updateOrderDto));
            // kiểm tra tồn tại
            var order = await _orderRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Order with ID {id} not found.");
            // cập nhật thông tin
            order.OrderCode = updateOrderDto.OrderCode ?? order.OrderCode;
            order.OrderDate = updateOrderDto.OrderDate ?? order.OrderDate;
            order.TotalAmount = updateOrderDto.TotalAmount ?? order.TotalAmount;
            order.CustomerId = updateOrderDto.CustomerId ?? order.CustomerId;
            var updatedOrder = await _orderRepository.UpdateAsync(order);
            return MapToResponse(updatedOrder);
        }

        private OrderResponseDto MapToResponse(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                CustomerId = order.CustomerId
            };
        }
    }
}
