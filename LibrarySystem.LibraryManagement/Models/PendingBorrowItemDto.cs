namespace LibrarySystem.API.Models
{
    /// <summary>
    /// Local DTO for pending borrow list response (same shape as Common.DTOs.Library.Borrows.PendingBorrowListItemDto).
    /// </summary>
    public class PendingBorrowItemDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? BookTitle { get; set; }
        public int BookCopyId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
    }
}
