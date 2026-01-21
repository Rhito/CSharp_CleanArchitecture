using CleanArchitecture.Application.DTOs.Product;
using CleanArchitecture.Application.Filters;
using CleanArchitecture.Application.IService;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }
            // Gọi Service để xử lý nghiệp vụ và lưu trữ
            var createdProduct = await _productService.CreateAsync(dto);
            // Trả về mã 201 kèm theo thông tin sản phẩm vừa tạo
            // Nó sẽ tạo ra một Header 'Location' trỏ tới hàm GetById
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] bool isHardDelete = false)
        {
            var result = await _productService.DeleteAsync(id, isHardDelete);
            if (!result)
            {
                return NotFound();
            }
            return Ok(new
            {
                code = 200,
                message = $"{(isHardDelete ? "Force" : "Soft")} delete successful for id {id}"
            });

        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ProductSearchFilter filter)
        {
            var result = await _productService.GetPagedProductsAsync(filter);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            var product = await _productService.GetByIdAsync(id) ?? throw new Exception($"Not found product with id {id}");
            if (dto == null)
            {
                return BadRequest();
            }

            var result = await _productService.UpdateAsync(id, dto);

            return Ok(result);
        }


        [HttpDelete("bulk-delete")]
        public async Task<IActionResult> BulkDelete([FromBody] List<int> ids, [FromQuery] bool isHardDelete = false)
        {
            var result = await _productService.BulkDeleteAsync(ids, isHardDelete);
            if (!result)
            {
                return NotFound();
            }
            return Ok(new
            {
                code = 200,
                message = $"{(isHardDelete ? "Force" : "Soft")} bulk delete successful for ids: {string.Join(", ", ids)}"
            });
        }

        [HttpPost("bulk-restore")]
        public async Task<IActionResult> BulkRestore([FromBody] List<int> ids)
        {
            var result = await _productService.BulkRestoreAsync(ids);
            if (!result)
            {
                return NotFound();
            }
            return Ok(new
            {
                code = 200,
                message = $"Bulk restore successful for ids: {string.Join(", ", ids)}"
            });
        }
        [HttpPost("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _productService.RestoreAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return Ok(new
            {
                code = 200,
                message = $"Restore successful for id {id}"
            });
        }
    }
}
