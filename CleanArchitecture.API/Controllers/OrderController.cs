using CleanArchitecture.Application.DTOs.Order;
using CleanArchitecture.Application.Filters;
using CleanArchitecture.Application.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;

        }

        // 1. GET: api/Order?keyword=abc&pageNumber=1&pageSize=10
        // Tìm kiếm và phân trang
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] OrderSearchFilter filter)
        {
            var result = await _orderService.SearchAsync(filter);
            return Ok(result);
        }

        // 2.GET: api/Order/5
        // Lấy chi tiết order theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            return Ok(order);
        }

        // 3.POST: api/Order
        // Tạo mới order
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var createdOrder = await _orderService.CreateOrderAsync(dto);
            // trả về 201 Created kèm header Location
            return Ok(createdOrder);
        }
        // 4.PUT: api/Order/5
        // Cập nhật thông tin order
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody]UpdateOrderDto dto)
        {
            var updateOrder = await _orderService.UpdateOrderAsync(id, dto);
            return Ok(new { message = $"Order with id {id} updated successfully." });
        }
        // 5.DELETE: api/Order/5
        // Xóa một order
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _orderService.DeleteOrderAsync(id);
            return Ok(new {message = $"Order with id {id} deleted successfully." });
        }
    }
}
