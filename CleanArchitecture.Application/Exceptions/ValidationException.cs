using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.Exceptions
{
    public class ValidationException : AppException
    {
        public ValidationException(string? message = null) : base(400, message ?? "Validation error 400!")
        {
        }
    
    }
}
