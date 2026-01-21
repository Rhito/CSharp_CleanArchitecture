using AutoMapper;
using CleanArchitecture.Application.DTOs.Category;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            // Cấu pháp: CreateMap<Nguồn, Đích>();

            // 1. Map từ Entity sang Response DTO (Dùng cho GetById, GetAll)
            // ReverseMap() giúp map ngược lại nếu cần
            CreateMap<Category, CategoryResponseDto>().ReverseMap();

            // 2. Map từ Create DTO sang Entity (Dùng cho Create)
            CreateMap<CreateCategoryDto, Category>();

            // 3. Map từ Update DTO sang Entity (Dùng cho Update)
            CreateMap<UpdateCategoryDto, Category>();

            // Ví dụ sau này cho Product:
            // CreateMap<Product, ProductResponseDto>().ReverseMap();
        }
    }
}
