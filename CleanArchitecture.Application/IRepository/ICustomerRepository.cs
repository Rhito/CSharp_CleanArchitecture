using CleanArchitecture.Application.Filters;
using CleanArchitecture.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.IRepository
{
    public interface ICustomerRepository
    {
        #region Read Operations (Đọc dữ liệu)

        /// <summary>
        /// Lấy thông tin khách hàng theo ID. Trả về null nếu không tìm thấy.
        /// </summary>
        Task<Customer?> GetByIdAsync(int id);

        /// <summary>
        /// Tìm kiếm và phân trang khách hàng theo bộ lọc.
        /// </summary>
        Task<IEnumerable<Customer>> SearchAsync(CustomerSearchFilter filter);

        /// <summary>
        /// Kiểm tra nhanh trạng thái đã xóa của một danh sách ID (Dùng để Validate).
        /// </summary>
        Task<List<(int Id, bool IsDeleted)>> GetListDeletedByIds(IEnumerable<int> ids);

        #endregion

        #region Write Operations (Thêm/Sửa)

        Task<Customer> AddAsync(Customer customer);

        /// <summary>
        /// Cập nhật thông tin khách hàng xuống Database.
        /// </summary>
        Task<Customer> UpdateAsync(Customer customer);

        #endregion

        #region Delete & Restore Operations (Xóa & Khôi phục)

        Task<bool> SoftDeleteAsync(int id);

        Task<bool> HardDeleteAsync(int id);

        Task<bool> RestoreAsync(int id);

        /// <summary>
        /// Xóa hàng loạt (Soft hoặc Hard tùy tham số).
        /// </summary>
        Task<bool> BulkDeleteAsync(IEnumerable<int> ids, bool isHardDelete);

        /// <summary>
        /// Khôi phục hàng loạt khách hàng đã xóa mềm.
        /// </summary>
        Task<bool> BulkRestoreAsync(IEnumerable<int> ids);

        #endregion
    }
}