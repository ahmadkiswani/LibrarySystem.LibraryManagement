-- Run this on LibraryDB (the database your LibraryManagement API uses).
-- Connection: same as appsettings.json DefaultConnection (e.g. Server=SedraPay;Database=LibraryDB;...)
--
-- This REMOVES the migration from history so that when you run "Update-Database"
-- again, EF Core will see it as pending and run it (adding the RoleName column).

USE [LibraryDB];
GO

DELETE FROM [dbo].[__EFMigrationsHistory]
WHERE [MigrationId] = N'20260203070000_AddRoleNameToUser';

-- If the row was there, you'll see "1 row affected". Then run in Package Manager Console:
--   Update-Database
-- (with LibrarySystem.LibraryManagement API project set as Startup Project,
--  and Default project = LibrarySystem.Domain)
GO
