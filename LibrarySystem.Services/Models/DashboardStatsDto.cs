namespace LibrarySystem.Services.Models
{
    public class DashboardStatsDto
    {
        public int TotalBooks { get; set; }
        public int ActiveUsers { get; set; }
        public int BorrowedToday { get; set; }
        public int Overdue { get; set; }
    }
}
