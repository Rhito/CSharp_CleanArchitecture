using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string? message = null) : base(404, message ?? "Resource not found 404!")
        {
        }
    }
}
