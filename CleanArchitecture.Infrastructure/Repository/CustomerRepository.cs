using CleanArchitecture.Application.Filters;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Thêm mới
        public async Task<Customer> AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        // 2. Update 
        // Logic: Nhận vào entity đã có dữ liệu mới -> Update -> Save
        public async Task<Customer> UpdateAsync(Customer customer)
        {
            // Đánh dấu entity là Modified để EF biết cần update
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        // 3. Get By Id (Sửa kiểu trả về cho phép null)
        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        // 4. Xóa cứng (Hard Delete)
        public async Task<bool> HardDeleteAsync(int id)
        {
            var rowsAffected = await _context.Customers
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync(); // EF Core 7+

            return rowsAffected > 0;
        }

        // 5. Xóa mềm (Soft Delete)
        public async Task<bool> SoftDeleteAsync(int id)
        {
            var rowsAffected = await _context.Customers
                .Where(c => c.Id == id)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.IsDeleted, true)); // EF Core 7+

            return rowsAffected > 0;
        }

        // 6. Khôi phục (Restore)
        public async Task<bool> RestoreAsync(int id)
        {
            var rowsAffected = await _context.Customers
                .Where(c => c.Id == id)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.IsDeleted, false));

            return rowsAffected > 0;
        }

        // 7. Xóa hàng loạt (Bulk Delete)
        public async Task<bool> BulkDeleteAsync(IEnumerable<int> ids, bool isHardDelete)
        {
            var query = _context.Customers.Where(c => ids.Contains(c.Id));

            if (isHardDelete)
            {
                return await query.ExecuteDeleteAsync() > 0;
            }

            return await query.ExecuteUpdateAsync(s => s.SetProperty(c => c.IsDeleted, true)) > 0;
        }

        // 8. Khôi phục hàng loạt
        public async Task<bool> BulkRestoreAsync(IEnumerable<int> ids)
        {
            return await _context.Customers
                .Where(c => ids.Contains(c.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.IsDeleted, false)) > 0;
        }

        // 9. Lấy danh sách trạng thái xóa (Validate nhanh)
        public async Task<List<(int Id, bool IsDeleted)>> GetListDeletedByIds(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
                return new List<(int, bool)>(); // Trả về list rỗng

            return await _context.Customers
                .Where(c => ids.Contains(c.Id))
                // Dùng cú pháp Tuple gọn hơn
                .Select(c => new ValueTuple<int, bool>(c.Id, c.IsDeleted))
                .ToListAsync();
        }

        // 10. Tìm kiếm
        public async Task<IEnumerable<Customer>> SearchAsync(CustomerSearchFilter filter)
        {
            // Luôn khởi tạo query AsNoTracking cho các tác vụ Tìm kiếm/Báo cáo để tối ưu
            var query = _context.Customers.AsNoTracking();

            // Nếu muốn tìm theo tên (ví dụ)
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(c => c.Name.Contains(filter.Keyword) || c.Email.Contains(filter.Keyword));
            }

            if (filter.FromCreatedAt.HasValue)
            {
                query = query.Where(c => c.CreatedAt >= filter.FromCreatedAt.Value);
            }

            if (filter.ToCreatedAt.HasValue)
            {
                query = query.Where(c => c.CreatedAt <= filter.ToCreatedAt.Value);
            }

            if (filter.IsDeleted.HasValue)
            {
                query = query.Where(c => c.IsDeleted == filter.IsDeleted.Value);
            }

            // Sắp xếp mặc định (nên có để phân trang ổn định)
            query = query.OrderByDescending(c => c.CreatedAt);

            // Phân trang
            int skip = (filter.PageNumber - 1) * filter.PageSize;

            return await query.Skip(skip).Take(filter.PageSize).ToListAsync();
        }
    }
}