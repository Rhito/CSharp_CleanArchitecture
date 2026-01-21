using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.Exceptions
{
    public class ServerErrorException : AppException
    {
        public ServerErrorException(string? message = null) : base(500, message ?? "Internal Server Error")
        {
        }
    }
}
