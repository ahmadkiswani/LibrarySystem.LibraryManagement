using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarySystem.Entities.Models
{
    public enum BorrowStatus
    {
        Pending = 0,
        Borrowed = 1,
        Returned = 2,
        Overdue = 3,
        Rejected = 4
    }

}
