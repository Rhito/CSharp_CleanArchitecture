using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CleanArchitecture.Application.DTOs.Category
{
    public class CreateCategoryDto
    {
        [Required, ]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
