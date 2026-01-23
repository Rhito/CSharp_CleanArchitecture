using CleanArchitecture.Application.DTOs.OrderDetail;
using CleanArchitecture.Application.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.IService
{
    public interface IOrderDetailService
    {
        Task<OrderDetailResponseDto> AddAsync(CreateOrderDetailDto dto);
        Task<OrderDetailResponseDto> UpdateAsync(int id ,UpdateOrderDetailDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<OrderDetailResponseDto> GetByIdAsync(int id);
        Task<IEnumerable<OrderDetailResponseDto>> SearchAsync(OrderDetailSearchFilter filter);
    }
}
