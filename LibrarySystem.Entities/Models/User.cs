using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Entities.Models
{
    public class User : AuditLog
    {

        [Required]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string UserName { get; set; }

        [Required, EmailAddress, MaxLength(50)]
        public string UserEmail { get; set; } = string.Empty;
        public int ExternalUserId { get; set; }
        public User? CreatedByUser { get; set; }
        public User? LastModifiedByUser { get; set; }
        public User? DeletedByUser { get; set; }
        public ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();
    }
}