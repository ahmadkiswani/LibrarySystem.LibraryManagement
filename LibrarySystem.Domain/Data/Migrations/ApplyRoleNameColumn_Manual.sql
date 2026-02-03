-- Run this script on the LibraryManagement database (LibraryDB on SedraPay).
-- Use the same connection as in LibrarySystem.LibraryManagement appsettings.json: DefaultConnection.
--
-- This adds the RoleName column expected by the app and records the migration so
-- "Update-Database" won't try to apply it again.

USE [LibraryDB];
GO

-- Add column if missing (safe to run multiple times)
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = N'RoleName'
)
BEGIN
    ALTER TABLE [dbo].[Users] ADD [RoleName] nvarchar(50) NULL;
    PRINT 'Column [RoleName] added to [dbo].[Users].';
END
ELSE
    PRINT 'Column [RoleName] already exists on [dbo].[Users].';

-- Record migration so EF Core considers it applied (avoids duplicate apply or errors)
IF NOT EXISTS (
    SELECT 1 FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = N'20260203070000_AddRoleNameToUser'
)
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260203070000_AddRoleNameToUser', N'10.0.2');
    PRINT 'Migration 20260203070000_AddRoleNameToUser recorded in __EFMigrationsHistory.';
END
ELSE
    PRINT 'Migration 20260203070000_AddRoleNameToUser already recorded.';

GO
