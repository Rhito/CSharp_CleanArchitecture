using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CleanArchitecture.Application.DTOs
{
    // Kế thừa IValidatableObject để viết logic validate phức tạp (chéo 2 trường)
    internal class ExampleCreateDto : IValidatableObject
    {
        // ==========================================
        // 1. NHÓM CHUỖI (STRING)
        // ==========================================

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Tên phải từ 5 đến 100 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(500, ErrorMessage = "Mô tả không được quá 500 ký tự.")]
        public string Description { get; set; } = string.Empty;

        // Regex: Bắt buộc mã phải theo định dạng "PROD-XXXX" (VD: PROD-1234)
        [RegularExpression(@"^PROD-\d{4}$", ErrorMessage = "Mã SKU phải có định dạng 'PROD-xxxx' (x là số).")]
        public string SkuCode { get; set; } = string.Empty;

        // ==========================================
        // 2. NHÓM ĐỊNH DẠNG ĐẶC BIỆT (FORMAT)
        // ==========================================

        [Required]
        [EmailAddress(ErrorMessage = "Email nhà cung cấp không hợp lệ.")]
        public string SupplierEmail { get; set; } = string.Empty;

        // Compare: So sánh trường này phải giống trường SupplierEmail
        [Compare("SupplierEmail", ErrorMessage = "Email xác nhận không khớp.")]
        public string ConfirmSupplierEmail { get; set; } = string.Empty;

        [Url(ErrorMessage = "Đường dẫn ảnh phải là một URL hợp lệ (http/https).")]
        public string? ImageUrl { get; set; }

        [Phone(ErrorMessage = "Số điện thoại hỗ trợ không hợp lệ.")]
        public string? SupportPhone { get; set; }

        // ==========================================
        // 3. NHÓM SỐ (NUMBER)
        // ==========================================

        // Validate tiền tệ (decimal)
        [Required]
        [Range(1000, (double)decimal.MaxValue, ErrorMessage = "Giá sản phẩm thấp nhất là 1,000đ.")]
        public decimal Price { get; set; }

        // Validate số nguyên (int)
        [Range(0, 100, ErrorMessage = "Số lượng tồn kho chỉ được từ 0 đến 100.")]
        public int StockQuantity { get; set; }

        // Validate điểm đánh giá (double)
        [Range(1.0, 5.0, ErrorMessage = "Rating chỉ từ 1 đến 5 sao.")]
        public double Rating { get; set; }

        // ==========================================
        // 4. NHÓM BOOLEAN & KHÁC
        // ==========================================

        // Bắt buộc phải là True (VD: Đồng ý điều khoản)
        [Range(typeof(bool), "true", "true", ErrorMessage = "Bạn phải cam kết sản phẩm chính hãng.")]
        public bool IsAuthentic { get; set; }

        // List không được rỗng (Custom check một chút ở dưới hoặc dùng thư viện ngoài)
        [MinLength(1, ErrorMessage = "Phải có ít nhất 1 tag.")]
        public List<string> Tags { get; set; } = new List<string>();

        // ==========================================
        // 5. CUSTOM LOGIC (VALIDATE CHÉO)
        // ==========================================

        // Hàm này tự động chạy sau khi các Attribute ở trên đã thỏa mãn
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Logic 1: Giá khuyến mãi không được lớn hơn giá gốc
            // (Giả sử bạn có thêm trường PromotionPrice)
            if (PromotionPrice.HasValue && PromotionPrice > Price)
            {
                yield return new ValidationResult(
                    "Giá khuyến mãi không được lớn hơn giá gốc.",
                    new[] { nameof(PromotionPrice) } // Chỉ định lỗi hiển thị ở trường nào
                );
            }

            // Logic 2: Nếu là hàng điện tử (Tag chứa "Electronics") thì bắt buộc phải có ImageUrl
            if (Tags.Contains("Electronics") && string.IsNullOrEmpty(ImageUrl))
            {
                yield return new ValidationResult(
                    "Sản phẩm điện tử bắt buộc phải có ảnh minh họa.",
                    new[] { nameof(ImageUrl) }
                );
            }
        }

        // Thêm trường giả để demo logic 1
        public decimal? PromotionPrice { get; set; }
    }
}
