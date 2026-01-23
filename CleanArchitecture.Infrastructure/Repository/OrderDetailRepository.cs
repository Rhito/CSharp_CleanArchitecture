using CleanArchitecture.Application.DTOs.OrderDetail;
using CleanArchitecture.Application.Filters;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Infrastructure.Repository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // create an new orderDetail record
        public async Task<OrderDetail> CreateAsync(OrderDetail od)
        {
            await _context.OrderDetails.AddAsync(od);
            await _context.SaveChangesAsync();
            return od;
        }

        // update
        public async Task<OrderDetail> UpdateAsync(OrderDetail od)
        {
             _context.OrderDetails.Update(od);
            await _context.SaveChangesAsync();
            return od;
        }

        // get by id
        public async Task<OrderDetail> GetByIdAsync(int id)
        {
            return await _context.OrderDetails.FindAsync(id) ?? throw new KeyNotFoundException();
        }

        public async Task<IEnumerable<OrderDetail>> SearchAsync(OrderDetailSearchFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            var query = _context.OrderDetails.AsNoTracking();

            query = query.OrderByDescending(x => x.Id);

            int skip = (filter.PageNumber - 1) * filter.PageSize;

            return await query.Skip(skip)
                      .Take(filter.PageSize)
                      .ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rowAffected = await _context.OrderDetails.Where(od => od.Id == id).ExecuteDeleteAsync();
            return rowAffected > 0;
        }
    }
}
