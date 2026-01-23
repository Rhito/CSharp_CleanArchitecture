using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.DTOs.Validation
{
    public class ValidationErrorResponseDto
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = default!;
        public object Errors { get; set; } = default!;
        public DateTime Timestamp = DateTime.UtcNow;
    }
}
