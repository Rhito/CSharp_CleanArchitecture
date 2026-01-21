
using CleanArchitecture.Application.Filters;
using CleanArchitecture.Domain.Entities;


namespace CleanArchitecture.Application.IRepository
{
    public interface IProductRepository
    {
        Task<bool> SoftDeleteAsync (int id);
        Task<bool> HardDeleteAsync (int id);
        Task<Product> GetByIdAsync(int id);
        Task<List<Product>> GetAllAsync();
        Task<Product> AddAsync(Product product);
        Task<Product> UpdateAsync(int id, Product product);
        Task<IEnumerable<Product>> SearchProductAsync(ProductSearchFilter productSearchFilter);
        Task<bool> BulkDeleteAsync(List<int> ids, bool isHardDelete);
        Task<bool> BulkRestoreAsync(List<int> ids);
        Task<bool> RestoreAsync(int id);
        Task<List<(int Id, bool IsDeleted)>> GetListDeletedByIds(List<int> ids);
    }
}
