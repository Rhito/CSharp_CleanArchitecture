using CleanArchitecture.Application.Filters;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<IEnumerable<Order>> SearchAsync(OrderSearchFilter filter)
        {
           var query = _context.Orders.AsQueryable();
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(o => o.OrderCode.Contains(filter.Keyword));
            }
            // Sắp xếp mặc định (nên có để phân trang ổn định)
            query = query.OrderByDescending(c => c.Id);
            return await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task<Order> UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rowsAffected = await _context.Orders
                .Where(o => o.Id == id)
                .ExecuteDeleteAsync();
            return rowsAffected > 0;
        }

    }
}
