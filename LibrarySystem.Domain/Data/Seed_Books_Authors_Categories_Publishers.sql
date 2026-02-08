-- ============================================================
-- Library seed: Authors, Categories, Publishers, Books
-- Run after migrations. Uses IDENTITY_INSERT for predictable IDs.
-- ============================================================

SET NOCOUNT ON;

-- ---------- 1. Categories ----------
SET IDENTITY_INSERT Categories ON;

INSERT INTO Categories (Id, Name, CreatedBy, CreatedDate, LastModifiedBy, LastModifiedDate, IsDeleted, DeletedBy, DeletedDate)
VALUES
(1, N'Fiction', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(2, N'Non-Fiction', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(3, N'Science', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(4, N'Technology', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(5, N'History', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(6, N'Biography', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(7, N'Children', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(8, N'Poetry', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(9, N'Business', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(10, N'Philosophy', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(11, N'Romance', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(12, N'Thriller', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(13, N'Fantasy', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(14, N'Education', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(15, N'Art & Design', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL);

SET IDENTITY_INSERT Categories OFF;

-- ---------- 2. Authors ----------
SET IDENTITY_INSERT Authors ON;

INSERT INTO Authors (Id, AuthorName, CreatedBy, CreatedDate, LastModifiedBy, LastModifiedDate, IsDeleted, DeletedBy, DeletedDate)
VALUES
(1, N'George Orwell', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(2, N'Jane Austen', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(3, N'Stephen King', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(4, N'Agatha Christie', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(5, N'J.K. Rowling', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(6, N'Isaac Asimov', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(7, N'Yuval Noah Harari', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(8, N'Bill Bryson', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(9, N'Malcolm Gladwell', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(10, N'Dale Carnegie', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(11, N'Paulo Coelho', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(12, N'Khaled Hosseini', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(13, N'Gabriel Garcia Marquez', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(14, N'Roald Dahl', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(15, N'Dr. Seuss', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(16, N'Robert Frost', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(17, N'Emily Dickinson', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(18, N'Plato', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(19, N'Friedrich Nietzsche', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(20, N'Dan Brown', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(21, N'John Grisham', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(22, N'J.R.R. Tolkien', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(23, N'Richard Dawkins', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(24, N'Stephen Hawking', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(25, N'Eric Ries', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL);

SET IDENTITY_INSERT Authors OFF;

-- ---------- 3. Publishers ----------
SET IDENTITY_INSERT Publishers ON;

INSERT INTO Publishers (Id, Name, CreatedBy, CreatedDate, LastModifiedBy, LastModifiedDate, IsDeleted, DeletedBy, DeletedDate)
VALUES
(1, N'Penguin Random House', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(2, N'HarperCollins', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(3, N'Simon & Schuster', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(4, N'Macmillan', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(5, N'Hachette', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(6, N'Oxford University Press', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(7, N'Cambridge University Press', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(8, N'O''Reilly Media', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(9, N'Addison-Wesley', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(10, N'Scholastic', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(11, N'Bloomsbury', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(12, N'Faber & Faber', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(13, N'Crown Publishing', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(14, N'Riverhead Books', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(15, N'Vintage', NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL);

SET IDENTITY_INSERT Publishers OFF;

-- ---------- 4. Books (AuthorId, CategoryId, PublisherId, Title, Version, PublishDate, TotalCopies, audit) ----------
SET IDENTITY_INSERT Books ON;

INSERT INTO Books (Id, Title, Version, PublishDate, TotalCopies, AuthorId, CategoryId, PublisherId, CreatedBy, CreatedDate, LastModifiedBy, LastModifiedDate, IsDeleted, DeletedBy, DeletedDate)
VALUES
-- Orwell (1) - Fiction, Non-Fiction
(1, N'1984', N'1st', '1949-06-08', 10, 1, 1, 1, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(2, N'Animal Farm', N'1st', '1945-08-17', 8, 1, 1, 1, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(3, N'Homage to Catalonia', N'1st', '1938-04-25', 5, 1, 2, 1, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Austen (2) - Fiction, Romance
(4, N'Pride and Prejudice', N'1st', '1813-01-28', 12, 2, 1, 2, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(5, N'Sense and Sensibility', N'1st', '1811-10-30', 7, 2, 11, 2, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(6, N'Emma', N'1st', '1815-12-23', 9, 2, 1, 2, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Stephen King (3) - Thriller, Fantasy
(7, N'The Shining', N'1st', '1977-01-28', 6, 3, 12, 3, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(8, N'It', N'1st', '1986-09-15', 8, 3, 12, 3, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(9, N'The Dark Tower I', N'1st', '1982-06-10', 5, 3, 13, 3, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Christie (4) - Fiction, Thriller
(10, N'Murder on the Orient Express', N'1st', '1934-01-01', 11, 4, 12, 4, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(11, N'Death on the Nile', N'1st', '1937-11-01', 7, 4, 12, 4, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(12, N'The ABC Murders', N'1st', '1936-01-01', 6, 4, 12, 4, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Rowling (5) - Fantasy, Children
(13, N'Harry Potter and the Philosopher''s Stone', N'1st', '1997-06-26', 15, 5, 13, 11, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(14, N'Harry Potter and the Chamber of Secrets', N'1st', '1998-07-02', 14, 5, 13, 11, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(15, N'Harry Potter and the Prisoner of Azkaban', N'1st', '1999-07-08', 13, 5, 13, 11, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(16, N'Harry Potter and the Goblet of Fire', N'1st', '2000-07-08', 12, 5, 7, 11, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Asimov (6) - Science, Fiction
(17, N'Foundation', N'1st', '1951-06-01', 9, 6, 3, 5, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(18, N'I, Robot', N'1st', '1950-12-02', 8, 6, 4, 5, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(19, N'The Gods Themselves', N'1st', '1972-01-01', 5, 6, 3, 5, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Harari (7) - History, Non-Fiction
(20, N'Sapiens', N'1st', '2011-01-01', 10, 7, 5, 14, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(21, N'Homo Deus', N'1st', '2015-01-01', 8, 7, 2, 14, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(22, N'21 Lessons for the 21st Century', N'1st', '2018-08-30', 7, 7, 2, 14, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Bryson (8) - Non-Fiction, Science
(23, N'A Short History of Nearly Everything', N'1st', '2003-05-06', 9, 8, 3, 2, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(24, N'The Body', N'1st', '2019-10-15', 6, 8, 3, 2, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(25, N'A Walk in the Woods', N'1st', '1998-05-04', 5, 8, 2, 2, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Gladwell (9) - Business, Non-Fiction
(26, N'Outliers', N'1st', '2008-11-18', 8, 9, 9, 3, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(27, N'The Tipping Point', N'1st', '2000-03-01', 7, 9, 9, 3, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(28, N'Blink', N'1st', '2005-01-11', 6, 9, 2, 3, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Carnegie (10) - Business
(29, N'How to Win Friends and Influence People', N'1st', '1936-10-01', 15, 10, 9, 13, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(30, N'How to Stop Worrying and Start Living', N'1st', '1948-01-01', 10, 10, 9, 13, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Coelho (11) - Fiction, Philosophy
(31, N'The Alchemist', N'1st', '1988-01-01', 20, 11, 1, 15, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(32, N'Brida', N'1st', '1990-01-01', 6, 11, 1, 15, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Hosseini (12) - Fiction
(33, N'The Kite Runner', N'1st', '2003-05-29', 11, 12, 1, 2, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(34, N'A Thousand Splendid Suns', N'1st', '2007-05-22', 9, 12, 1, 2, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Marquez (13) - Fiction
(35, N'One Hundred Years of Solitude', N'1st', '1967-05-30', 8, 13, 1, 15, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(36, N'Love in the Time of Cholera', N'1st', '1985-09-01', 7, 13, 11, 15, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Dahl (14) - Children
(37, N'Charlie and the Chocolate Factory', N'1st', '1964-01-17', 12, 14, 7, 10, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(38, N'Matilda', N'1st', '1988-10-01', 10, 14, 7, 10, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(39, N'The BFG', N'1st', '1982-01-01', 8, 14, 7, 10, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Dr. Seuss (15) - Children
(40, N'The Cat in the Hat', N'1st', '1957-03-12', 14, 15, 7, 10, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(41, N'Green Eggs and Ham', N'1st', '1960-08-12', 16, 15, 7, 10, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Frost (16) - Poetry
(42, N'North of Boston', N'1st', '1914-02-01', 5, 16, 8, 12, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(43, N'New Hampshire', N'1st', '1923-01-01', 4, 16, 8, 12, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Dickinson (17) - Poetry
(44, N'The Poems of Emily Dickinson', N'1st', '1890-01-01', 6, 17, 8, 12, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Plato (18) - Philosophy
(45, N'The Republic', N'1st', '2000-01-01', 9, 18, 10, 6, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(46, N'Symposium', N'1st', '2000-01-01', 5, 18, 10, 6, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Nietzsche (19) - Philosophy
(47, N'Thus Spoke Zarathustra', N'1st', '1883-01-01', 6, 19, 10, 7, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(48, N'Beyond Good and Evil', N'1st', '1886-01-01', 5, 19, 10, 7, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Dan Brown (20) - Thriller
(49, N'The Da Vinci Code', N'1st', '2003-03-18', 13, 20, 12, 3, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(50, N'Angels & Demons', N'1st', '2000-05-01', 10, 20, 12, 3, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Grisham (21) - Thriller
(51, N'The Firm', N'1st', '1991-02-01', 8, 21, 12, 4, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(52, N'A Time to Kill', N'1st', '1989-06-01', 7, 21, 12, 4, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Tolkien (22) - Fantasy
(53, N'The Hobbit', N'1st', '1937-09-21', 14, 22, 13, 2, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(54, N'The Lord of the Rings', N'1st', '1954-07-29', 11, 22, 13, 2, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Dawkins (23) - Science
(55, N'The Selfish Gene', N'1st', '1976-01-01', 9, 23, 3, 6, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(56, N'The God Delusion', N'1st', '2006-10-02', 8, 23, 3, 6, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Hawking (24) - Science
(57, N'A Brief History of Time', N'1st', '1988-04-01', 12, 24, 3, 4, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(58, N'The Universe in a Nutshell', N'1st', '2001-11-01', 7, 24, 3, 4, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
-- Eric Ries (25) - Business, Technology
(59, N'The Lean Startup', N'1st', '2011-09-13', 10, 25, 9, 8, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL),
(60, N'The Startup Way', N'1st', '2017-10-17', 6, 25, 4, 8, NULL, GETUTCDATE(), NULL, NULL, 0, NULL, NULL);

SET IDENTITY_INSERT Books OFF;

PRINT N'Seed completed: 15 Categories, 25 Authors, 15 Publishers, 60 Books.';
