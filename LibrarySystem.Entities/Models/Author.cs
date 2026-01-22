using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Entities.Models
{
    public class Author : AuditLog
    {
        [Required]
        public int Id { get; set; } 


        [Required]
        [MaxLength(50)]
        public string AuthorName { get; set; } 

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
