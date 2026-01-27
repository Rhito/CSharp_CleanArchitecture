using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Data.Entities
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expires {  get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public int? RevokedById { get; set; }
        public string? ReplacedByToken { get; set; }
        public string? ReasonRevoked { get; set; }

        [NotMapped]
        public bool IsExprires => DateTime.UtcNow > Expires;

        [NotMapped]
        public bool IsRevoked => Revoked != null;
        [NotMapped]
        public bool IsActive => !IsRevoked && !IsExprires;

        public string UserId { get; set; } = string.Empty;
    }
}
