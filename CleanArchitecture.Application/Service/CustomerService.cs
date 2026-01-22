using CleanArchitecture.Application.DTOs.Customer;
using CleanArchitecture.Application.Exceptions;
using CleanArchitecture.Application.Filters;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Application.IService;
using CleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repo;

        public CustomerService(ICustomerRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> BulkDeleteAsync(List<int> ids, bool isHardDelete = false)
        {
            // 1. Validate Input
            if (ids == null || !ids.Any())
            {
                throw new ArgumentException("IDs list cannot be null or empty.");
            }

            // 2. Loại bỏ trùng lặp
            var distinctIds = ids.Distinct().ToList();

            // 3. Kiểm tra sự tồn tại (Logic nghiệp vụ: Phải tồn tại hết mới cho xóa)
            var existingCustomers = await _repo.GetListDeletedByIds(distinctIds);

            if (existingCustomers.Count != distinctIds.Count)
            {
                throw new NotFoundException("Some customer IDs do not exist.");
            }

            // 4. Kiểm tra xem có ai đã bị xóa rồi không (nếu xóa mềm)
            // Logic: Nếu đang định xóa mềm mà user đó đã xóa rồi -> Báo lỗi conflict
            if (!isHardDelete && existingCustomers.Any(c => c.IsDeleted))
            {
                throw new AppConflictException("Some customers are already deleted.");
            }

            // 5. Gọi Repo
            return await _repo.BulkDeleteAsync(distinctIds, isHardDelete);
        }

        public async Task<bool> BulkRestoreAsync(List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                throw new ArgumentException("IDs list cannot be null or empty.");
            }

            var distinctIds = ids.Distinct().ToList();

            // Có thể thêm logic kiểm tra xem ID có tồn tại không tương tự hàm Delete nếu cần thiết

            return await _repo.BulkRestoreAsync(distinctIds);
        }

        public async Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto)
        {
            // 1. Guard Clause
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // 2. Map DTO -> Entity
            var customer = new Customer
            {
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Address = dto.Address,
                IsDeleted = dto.IsDeleted,
                CreatedAt = DateTime.Now
            };

            // 3. Save
            var savedCustomer = await _repo.AddAsync(customer);

            // 4. Map Entity -> Response
            return MapToResponse(savedCustomer);
        }

        public async Task<bool> DeleteAsync(int id, bool isHardDelete)
        {
            // Kiểm tra tồn tại
            var customer = await _repo.GetByIdAsync(id);
            if (customer == null)
            {
                throw new NotFoundException($"Could not find customer with id {id}");
            }

            // Gọi hàm tương ứng
            if (isHardDelete)
                return await _repo.HardDeleteAsync(id);

            return await _repo.SoftDeleteAsync(id);
        }

        public async Task<CustomerResponseDto> GetByIdAsync(int id)
        {
            var customer = await _repo.GetByIdAsync(id);

            if (customer == null)
                throw new NotFoundException($"Could not find customer with id {id}");

            return MapToResponse(customer);
        }

        public async Task<IEnumerable<CustomerResponseDto>> GetPagedCustomersAsync(CustomerSearchFilter filter)
        {
            // SỬA: Đổi tên hàm từ SearchProductAsync -> SearchAsync
            var customers = await _repo.SearchAsync(filter);

            // Map danh sách
            return customers.Select(c => MapToResponse(c));
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var customer = await _repo.GetByIdAsync(id);

            if (customer == null)
                throw new NotFoundException($"Could not find customer with id {id}");

            if (!customer.IsDeleted)
            {
                throw new ValidationException($"Customer with id {id} is not currently deleted.");
            }

            return await _repo.RestoreAsync(id);
        }

        public async Task<CustomerResponseDto> UpdateAsync(int id, UpdateCustomerDto dto)
        {
            // 1. Get
            var customer = await _repo.GetByIdAsync(id);
            if (customer == null)
                throw new NotFoundException($"Could not find customer with id {id}");

            // 2. Update Logic (Partial Update)
            customer.Name = dto.Name ?? customer.Name;
            customer.Email = dto.Email ?? customer.Email;
            customer.PhoneNumber = dto.PhoneNumber ?? customer.PhoneNumber;
            customer.Address = dto.Address ?? customer.Address;
            customer.IsDeleted = dto.IsDeleted;

            // 3. Save (SỬA: Repo update chỉ nhận entity, không nhận ID)
            var updatedCustomer = await _repo.UpdateAsync(customer);

            // 4. Return (SỬA: Thêm return)
            return MapToResponse(updatedCustomer);
        }

        // --- Helper Method để tránh lặp lại code Map ---
        private static CustomerResponseDto MapToResponse(Customer customer)
        {
            return new CustomerResponseDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                CreatedAt = customer.CreatedAt,
                IsDeleted = customer.IsDeleted
            };
        }
    }
}