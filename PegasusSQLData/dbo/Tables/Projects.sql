CREATE TABLE [dbo].[Projects]
(
	[Id]            INT          NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [Name]          NVARCHAR(50) NULL, 
    [ProjectPrefix] NVARCHAR(20) NULL, 
    [IsPinned] BIT NOT NULL DEFAULT 0, 
    [IsActive] BIT NOT NULL DEFAULT 1
)
