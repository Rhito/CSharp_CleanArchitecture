using CleanArchitecture.Application.DTOs.Customer;
using CleanArchitecture.Application.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.IService
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerResponseDto>> GetPagedCustomersAsync(CustomerSearchFilter filter);
        Task<CustomerResponseDto> GetByIdAsync(int id);
        Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto);
        Task<CustomerResponseDto> UpdateAsync(int id, UpdateCustomerDto dto);
        Task<bool> DeleteAsync(int id, bool isHardDelete);
        Task<bool> RestoreAsync(int id);
        Task<bool> BulkDeleteAsync(List<int> ids, bool isHardDelete);
        Task<bool> BulkRestoreAsync(List<int> ids);

    }
}
