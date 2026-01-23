using CleanArchitecture.Application.DTOs.Customer;
using CleanArchitecture.Application.Filters;
using CleanArchitecture.Application.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // 1. GET: api/Customer?keyword=abc&pageNumber=1&pageSize=10
        // Tìm kiếm và phân trang
        [HttpGet]
        public async Task<IActionResult> GetCustomers([FromQuery] CustomerSearchFilter filter)
        {
            var result = await _customerService.GetPagedCustomersAsync(filter);
            return Ok(result);
        }
        // 2.GET: api/Customer/5
        // Lấy chi tiết khách hàng theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            return Ok(customer);
        }

        // 3.POST: api/Customer
        // Tạo mới khách hàng
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            var createdCustomer = await _customerService.CreateAsync(dto);

            // trả về 201 Created kèm header Location
            return Ok(createdCustomer);
        }

        // 4.PUT: api/Customer/5
        // Cập nhật thông tin khách hàng
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerDto dto)
        {
            var updatedCustomer = await _customerService.UpdateAsync(id, dto);
            return Ok(new { message = $"Customer with id {id} updated successfully." });

        }

        // 5.DELETE: api/Customer/5
        // Xóa một khách hàng (mềm hoặc cứng)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id, [FromQuery] bool isHardDelete = false)
        {
            await _customerService.DeleteAsync(id, isHardDelete);
            return Ok(new { message = $"Customer with id {id} deleted successfully." });
        }

        // 6.POST: api/Customer/bulk-delete
        // Xóa hàng loạt khách hàng
        [HttpPost("bulk-delete")]
        public async Task<IActionResult> BulkDeleteCustomers([FromBody] List<int> ids, [FromQuery] bool isHardDelete = false)
        {
            await _customerService.BulkDeleteAsync(ids, isHardDelete);
            return Ok(new { message = "Bulk delete operation completed successfully." });
        }

        // 7.POST: api/Customer/bulk-restore
        // Khôi phục hàng loạt khách hàng
        [HttpPost("bulk-restore")]
        public async Task<IActionResult> BulkRestoreCustomers([FromBody] List<int> ids)
        {
            await _customerService.BulkRestoreAsync(ids);
            return Ok(new { message = "Bulk restore operation completed successfully." });
        }

        // 8.POST: api/Customer/5/restore
        // Khôi phục một khách hàng
        [HttpPost("{id}/restore")]
        public async Task<IActionResult> RestoreCustomer(int id)
        {
            await _customerService.RestoreAsync(id);
            return Ok(new { message = $"Customer with id {id} restored successfully." });
        }
    }
}
