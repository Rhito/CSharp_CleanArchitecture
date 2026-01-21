using CleanArchitecture.Application.DTOs.Product;
using CleanArchitecture.Application.Filters;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Application.Exceptions;

namespace CleanArchitecture.Application.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository repository) {
            _productRepository = repository;
        }

        public async Task<ProductResponseDto> CreateAsync(CreateProductDto createProductAsyncDto)
        {

            var product = new Product
            {
                Name = createProductAsyncDto.Name,
                Description = createProductAsyncDto.Description,
                Price = createProductAsyncDto.Price,
                StockQuantity = createProductAsyncDto.StockQuantity,
                CategoryId = createProductAsyncDto.CategoryId
            };

            var savedProduct = await _productRepository.AddAsync(product);

            return new ProductResponseDto
            {
                Id = savedProduct.Id,
                Name = savedProduct.Name,
                Description = savedProduct.Description,
                Price = savedProduct.Price,
                StockQuantity = savedProduct.StockQuantity,
                CategoryId = savedProduct.CategoryId,
            };

        }

        public async Task<bool> DeleteAsync(int id, bool isHardDelete)
        {
            var product = await _productRepository.GetByIdAsync(id) ??
                          throw new NotFoundException($"Could not find product with id {id}");
            if (isHardDelete)
            {
                return await _productRepository.HardDeleteAsync(id);
            }

            if (product.IsDeleted.GetValueOrDefault())
            {
                throw new AppConflictException($"Product with id {id} is already deleted!");
            }

            
            return await _productRepository.SoftDeleteAsync(id);           
        }

        public async Task<ProductResponseDto> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            return product == null
                ? throw new NotFoundException($"Could not find product with id {id}")
                : new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                IsDeleted = product.IsDeleted,
            };
        }

        public async Task<IEnumerable<ProductResponseDto>> GetPagedProductsAsync(ProductSearchFilter filter)
        {
            // 1. Gọi Repo để lấy dữ liệu (Phần logic lọc sẽ nằm ở dưới Repo)
            var products = await _productRepository.SearchProductAsync(filter);
            // 2. Manual Mapping từ List Entity -> List DTO
            var response = products.Select(products => new ProductResponseDto
            {
                Id = products.Id,
                Name = products.Name,
                Description = products.Description,
                Price = products.Price,
                StockQuantity = products.StockQuantity,
                CategoryId = products.CategoryId,
                IsDeleted = products.IsDeleted,
            });

            return response;

        }

        public async Task<ProductResponseDto> UpdateAsync(int id, UpdateProductDto updateDto)
        {
            var product = await _productRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Could not find product with id {id}");

            product.Name = updateDto.Name ?? product.Name;
            product.Description = updateDto.Description ?? product.Description;
            product.Price = updateDto.Price ?? product.Price;
            product.StockQuantity = updateDto.StockQuantity ?? product.StockQuantity;
            product.CategoryId = updateDto.CategoryId ?? product.CategoryId;
            product.IsDeleted = updateDto.IsDeleted ?? product.IsDeleted;

            var updatedProduct = await _productRepository.UpdateAsync(id, product);

            return new ProductResponseDto
            {
                Id = updatedProduct.Id,
                Name = updatedProduct.Name,
                Description = updatedProduct.Description,
                Price = updatedProduct.Price,
                StockQuantity = updatedProduct.StockQuantity,
                CategoryId = updatedProduct.CategoryId,
                IsDeleted = updatedProduct.IsDeleted,
            };
        }

        public async Task<bool> BulkDeleteAsync(List<int> ids, bool isHardDelete)
        {
            if (ids == null || !ids.Any())
            {
                throw new ValidationException("IDs in product list cannot be null.");
            }
            var distinctIds = ids.Distinct().ToList();
            var productsStatus = await _productRepository.GetListDeletedByIds(distinctIds);

            if (productsStatus.Count != distinctIds.Count)
            {
                throw new NotFoundException("Some products do not exist.");
            }
            if(productsStatus.Any(p => p.IsDeleted))
            {
                throw new AppConflictException("Some products are already deleted.");
            }
            return await _productRepository.BulkDeleteAsync(distinctIds, isHardDelete);

        }

        public async Task<bool> BulkRestoreAsync(List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                throw new ValidationException("IDs in product list cannot be null.");
            }
            var distinctIds = ids.Distinct().ToList();

            return await _productRepository.BulkRestoreAsync(distinctIds);
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id) ??
                          throw new NotFoundException($"Could not find product with id {id}");
            if (product.IsDeleted == false)
            {
                throw new ValidationException($"Product with id {id} is not deleted!");
            }
            return await _productRepository.RestoreAsync(id);
        }
    }
}
