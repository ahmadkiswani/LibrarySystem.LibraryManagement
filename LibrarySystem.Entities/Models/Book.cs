
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Entities.Models
{
    public class Book : AuditLog
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        [MaxLength(50)]
        public DateTime PublishDate { get; set; }
        [MaxLength(50)]
        public string Version { get; set; }
        public int TotalCopies { get;  set; } 
        public int AuthorId { get; set; }
        public Author?Author { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int PublisherId { get; set; }
        public Publisher? Publisher { get; set; }
        public ICollection<BookCopy> Copies { get; set; } = new List<BookCopy>();
 }

}

