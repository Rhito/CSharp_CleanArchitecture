using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.Exceptions
{
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string? message = null) : base(401, message ?? "Unauthorized 403!")
        {
        }
    }
}
