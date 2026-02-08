-- ============================================================
-- سيد نسخ الكتب: 5 نسخ لكل كتاب
-- Run after Seed_Books_Authors_Categories_Publishers.sql (and optionally Delete_Borrows_And_BookCopies.sql if you want a clean start).
-- ============================================================

SET NOCOUNT ON;

-- إذا بدك تمسح النسخ والاستعارات اللي قبل وتعيد من جديد، شغّل الملف:
--   Delete_Borrows_And_BookCopies.sql
-- بعدين شغّل هذا الملف.

-- ---------- إدراج 5 نسخ لكل كتاب (من جدول Books) ----------
INSERT INTO BookCopies (BookId, AuthorId, CategoryId, PublisherId, CopyCode, IsAvailable, IsDeleted, CreatedBy, CreatedDate, LastModifiedBy, LastModifiedDate, DeletedBy, DeletedDate)
SELECT
    b.Id,
    b.AuthorId,
    b.CategoryId,
    b.PublisherId,
    N'BOOK-' + CAST(b.Id AS NVARCHAR(10)) + N'-C' + CAST(n.n AS NVARCHAR(2)),
    1,
    0,
    NULL,
    GETUTCDATE(),
    NULL,
    NULL,
    NULL,
    NULL
FROM Books b
CROSS JOIN (SELECT 1 AS n UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4 UNION ALL SELECT 5) n
WHERE b.IsDeleted = 0;

PRINT N'Seed completed: 5 copies per book (' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' BookCopies).';
