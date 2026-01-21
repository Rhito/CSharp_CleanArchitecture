using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.Exceptions
{
    public class AppConflictException : AppException
    {
        public AppConflictException(string? message = null) : base(409, message ?? "Invalid data conflict 409!")
        {
        }
    }
}
