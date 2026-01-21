using CleanArchitecture.Application.DTOs.Category;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        // 1. Bỏ tham số IMapper trong Constructor
        public CategoryService(ICategoryRepository repo)
        {
            _categoryRepository = repo;
        }

        public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto createCategoryDto)
        {
            if (createCategoryDto == null)
            {
                throw new ArgumentNullException(nameof(createCategoryDto), "Category data must not be null.");
            }

            // 2. Map thủ công từ DTO -> Entity
            var category = new Category
            {
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description
            };

            var createdCategory = await _categoryRepository.AddAsync(category);

            // 3. Map thủ công từ Entity -> Response DTO
            return new CategoryResponseDto
            {
                Id = createdCategory.Id,
                Name = createdCategory.Name,
                Description = createdCategory.Description
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var isDeleted = await _categoryRepository.DeleteAsync(id);

            if (!isDeleted)
            {
                throw new KeyNotFoundException($"Category with id {id} not found.");
            }

            return true;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllCategory();

            // 4. Dùng LINQ Select để map cả danh sách
            return categories.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            });
        }

        public async Task<CategoryResponseDto> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            // 5. Map thủ công kết quả trả về
            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<CategoryResponseDto> UpdateAsync(int id, UpdateCategoryDto updateCategoryDto)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);

            // Logic Merge dữ liệu (Giữ nguyên như cũ vì phần này bạn làm tay rất tốt)
            existingCategory.Name = updateCategoryDto.Name ?? existingCategory.Name;
            existingCategory.Description = updateCategoryDto.Description ?? existingCategory.Description;

            var updatedCategory = await _categoryRepository.UpdateAsync(id, existingCategory);

            // 6. Map kết quả trả về
            return new CategoryResponseDto
            {
                Id = updatedCategory.Id,
                Name = updatedCategory.Name,
                Description = updatedCategory.Description
            };
        }
    }
}