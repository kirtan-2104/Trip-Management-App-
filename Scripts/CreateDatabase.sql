-- ============================================
-- TripMate Database Creation Script
-- SQL Server
-- ============================================
-- Run this script to create the TripMate database and tables.
-- Usage: Execute in SQL Server Management Studio or sqlcmd
-- ============================================

USE master;
GO

-- Create database if it does not exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'TripMate')
BEGIN
    CREATE DATABASE [TripMate];
END
GO

USE [TripMate];
GO

-- ============================================
-- Drop tables if they exist (for clean re-create)
-- Remove this block if you want to preserve existing data
-- ============================================
IF OBJECT_ID(N'dbo.ExpenseSplits', N'U') IS NOT NULL DROP TABLE dbo.ExpenseSplits;
IF OBJECT_ID(N'dbo.Expenses', N'U') IS NOT NULL DROP TABLE dbo.Expenses;
IF OBJECT_ID(N'dbo.TripMembers', N'U') IS NOT NULL DROP TABLE dbo.TripMembers;
IF OBJECT_ID(N'dbo.Trips', N'U') IS NOT NULL DROP TABLE dbo.Trips;
IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL DROP TABLE dbo.Users;
GO

-- ============================================
-- Users
-- ============================================
CREATE TABLE [dbo].[Users] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [Name] NVARCHAR(256) NOT NULL,
    [Email] NVARCHAR(256) NOT NULL,
    [PasswordHash] NVARCHAR(256) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT (GETUTCDATE()),
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]);
GO

-- ============================================
-- Trips
-- ============================================
CREATE TABLE [dbo].[Trips] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [Name] NVARCHAR(256) NOT NULL,
    [Destination] NVARCHAR(256) NOT NULL,
    [StartDate] DATETIME2 NOT NULL,
    [EndDate] DATETIME2 NOT NULL,
    [Status] INT NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT (GETUTCDATE()),
    CONSTRAINT [PK_Trips] PRIMARY KEY ([Id])
);
GO

-- ============================================
-- TripMembers
-- ============================================
CREATE TABLE [dbo].[TripMembers] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [TripId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [JoinedAt] DATETIME2 NOT NULL DEFAULT (GETUTCDATE()),
    CONSTRAINT [PK_TripMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TripMembers_Trips_TripId] FOREIGN KEY ([TripId]) REFERENCES [dbo].[Trips] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TripMembers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [UQ_TripMembers_TripId_UserId] UNIQUE ([TripId], [UserId])
);
GO

CREATE INDEX [IX_TripMembers_TripId] ON [dbo].[TripMembers] ([TripId]);
CREATE INDEX [IX_TripMembers_UserId] ON [dbo].[TripMembers] ([UserId]);
GO

-- ============================================
-- Expenses
-- ============================================
CREATE TABLE [dbo].[Expenses] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [TripId] INT NOT NULL,
    [Title] NVARCHAR(256) NOT NULL,
    [Amount] DECIMAL(18,2) NOT NULL,
    [PaidByUserId] INT NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT (GETUTCDATE()),
    [Note] NVARCHAR(MAX) NULL,
    [SplitType] INT NOT NULL,
    CONSTRAINT [PK_Expenses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Expenses_Trips_TripId] FOREIGN KEY ([TripId]) REFERENCES [dbo].[Trips] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Expenses_Users_PaidByUserId] FOREIGN KEY ([PaidByUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_Expenses_TripId] ON [dbo].[Expenses] ([TripId]);
CREATE INDEX [IX_Expenses_PaidByUserId] ON [dbo].[Expenses] ([PaidByUserId]);
GO

-- ============================================
-- ExpenseSplits
-- ============================================
CREATE TABLE [dbo].[ExpenseSplits] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [ExpenseId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [ShareAmount] DECIMAL(18,2) NOT NULL,
    CONSTRAINT [PK_ExpenseSplits] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ExpenseSplits_Expenses_ExpenseId] FOREIGN KEY ([ExpenseId]) REFERENCES [dbo].[Expenses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ExpenseSplits_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_ExpenseSplits_ExpenseId] ON [dbo].[ExpenseSplits] ([ExpenseId]);
CREATE INDEX [IX_ExpenseSplits_UserId] ON [dbo].[ExpenseSplits] ([UserId]);
GO

PRINT 'TripMate database created successfully.';
GO
