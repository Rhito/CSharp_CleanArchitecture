using CleanArchitecture.Application.DTOs.Order;
using CleanArchitecture.Application.Filters;
using CleanArchitecture.Application.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controller
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

        // 1. GET: api/Customer?keyword=abc&pageNumber=1&pageSize=10
        // Tìm kiếm và phân trang
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] OrderSearchFilter filter)
        {
            var result = await _orderService.SearchAsync(filter);
            return Ok(result);
        }

        // 2.GET: api/Customer/5
        // Lấy chi tiết khách hàng theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            return Ok(order);
        }

        // 3.POST: api/Customer
        // Tạo mới khách hàng

        // 4.PUT: api/Customer/5
        // Cập nhật thông tin khách hàng
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody]UpdateOrderDto dto)
        {
            var updateOrder = await _orderService.UpdateOrderAsync(id, dto);
            return Ok(new { message = $"Order with id {id} updated successfully." });
        }
        // 5.DELETE: api/Customer/5
        // Xóa một khách hàng
    }
}
