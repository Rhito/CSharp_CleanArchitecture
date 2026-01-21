using CleanArchitecture.Application.DTOs.Product;
using CleanArchitecture.Application.Filters;

namespace CleanArchitecture.Application.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetPagedProductsAsync(ProductSearchFilter filter);
        Task<ProductResponseDto> GetByIdAsync(int id);
        Task<ProductResponseDto> CreateAsync(CreateProductDto createProductDto);
        Task<ProductResponseDto> UpdateAsync(int id, UpdateProductDto updateProductDto);
        Task<bool> DeleteAsync (int id, bool isHardDelete);
        Task<bool> RestoreAsync(int id);
        Task<bool> BulkDeleteAsync(List<int> ids, bool isHardDelete);
        Task<bool> BulkRestoreAsync(List<int> ids);

    }
}
