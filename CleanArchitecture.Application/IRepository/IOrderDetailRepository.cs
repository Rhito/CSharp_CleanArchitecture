using CleanArchitecture.Application.Filters;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Application.DTOs.OrderDetail;

namespace CleanArchitecture.Application.IRepository
{
    public interface IOrderDetailRepository
    {
        Task<OrderDetail> GetByIdAsync(int id);
        Task<IEnumerable<OrderDetail>> SearchAsync(OrderDetailSearchFilter filter);
        Task<OrderDetail> CreateAsync(OrderDetail od);
        Task<OrderDetail>UpdateAsync(OrderDetail od);
        Task<bool> DeleteAsync(int id);
    }
}
