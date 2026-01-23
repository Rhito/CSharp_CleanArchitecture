using CleanArchitecture.Application.DTOs.OrderDetail;
using CleanArchitecture.Application.Filters;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Application.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CleanArchitecture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController(IOrderDetailService orderDetailService) : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService = orderDetailService;

        // search and paginating
        [HttpGet]
        public async Task<IActionResult> GetOrderDetail([FromQuery]OrderDetailSearchFilter filter)
        {
            var orderDetails = await _orderDetailService.SearchAsync(filter);
            var response = ApiResponse<IEnumerable<OrderDetailResponseDto>>.Success(orderDetails);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var orderDetail = await _orderDetailService.GetByIdAsync(id);
            var response = ApiResponse<OrderDetailResponseDto>.Success(orderDetail);
            return Ok(response);
        }

        // add new record
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateOrderDetailDto createOrderDetailDto)
        {
            var orderDetail = await _orderDetailService.AddAsync(createOrderDetailDto);
            var response = ApiResponse<OrderDetailResponseDto>.Success(orderDetail, "Create successfully");
            return Ok(response);
        }

        // Update record
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]UpdateOrderDetailDto updateOrderDetailDto)
        {
            var orderDetail = await _orderDetailService.UpdateAsync(id, updateOrderDetailDto);
            var response = ApiResponse<OrderDetailResponseDto>.Success(orderDetail, "Update successfully");
            return Ok(response);
        }

        // Delete record
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var rowAffected = await _orderDetailService.DeleteAsync(id);
            var response = ApiResponse<bool>.Success(rowAffected,"Order Details delete successfully.");
            return Ok(response);
        }
    }
}
