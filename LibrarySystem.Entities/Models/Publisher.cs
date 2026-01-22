using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Entities.Models
{
    public class Publisher : AuditLog
    {

        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();


    }
}
