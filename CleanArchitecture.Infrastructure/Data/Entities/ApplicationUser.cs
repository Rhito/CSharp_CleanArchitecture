using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Infrastructure.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
       public string FullName { get; set; } = string.Empty;

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];
       
    }
}
