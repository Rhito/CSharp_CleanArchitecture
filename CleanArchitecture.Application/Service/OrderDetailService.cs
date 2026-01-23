using CleanArchitecture.Application.DTOs.OrderDetail;
using CleanArchitecture.Application.Filters;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.Service
{
    public class OrderDetailService(IOrderDetailRepository orderDetailRepository) : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository = orderDetailRepository;

        public async Task<OrderDetailResponseDto> AddAsync(CreateOrderDetailDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var orderDetail = new OrderDetail
            {
                OrderId = dto.OrderId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
            };
            var response = await _orderDetailRepository.CreateAsync(orderDetail);

            return MapToResponse(response);
        }

        public async Task<OrderDetailResponseDto> UpdateAsync(int id, UpdateOrderDetailDto updateDto)
        {
            var existsOrderDetail = await _orderDetailRepository.GetByIdAsync(id);     
            ArgumentNullException.ThrowIfNull(existsOrderDetail);

            existsOrderDetail.Quantity = updateDto.Quantity ?? existsOrderDetail.Quantity;
            existsOrderDetail.UnitPrice = updateDto.UnitPrice ?? existsOrderDetail.UnitPrice;

            await _orderDetailRepository.UpdateAsync(existsOrderDetail);
            return MapToResponse(existsOrderDetail);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existsOrderDetail = await _orderDetailRepository.GetByIdAsync(id);
            ArgumentNullException.ThrowIfNull(existsOrderDetail);
            return await _orderDetailRepository.DeleteAsync(id);
        }

        public async Task<OrderDetailResponseDto> GetByIdAsync(int id)
        {
            var existsOrderDetail = await _orderDetailRepository.GetByIdAsync(id);
            ArgumentNullException.ThrowIfNull(existsOrderDetail);
            return MapToResponse(existsOrderDetail);
        }

        public async Task<IEnumerable<OrderDetailResponseDto>> SearchAsync(OrderDetailSearchFilter filter)
        {
            var response = await _orderDetailRepository.SearchAsync(filter);
            return response.Select(od => MapToResponse(od));
        }

        private static OrderDetailResponseDto MapToResponse(OrderDetail response)
        {
            return new OrderDetailResponseDto
            {
                Id = response.Id,
                OrderId = response.OrderId,
                ProductId = response.ProductId,
                Quantity = response.Quantity,
                UnitPrice = response.UnitPrice,
            };
        }
    }
}
