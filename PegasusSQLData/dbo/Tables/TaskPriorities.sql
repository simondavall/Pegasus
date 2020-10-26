CREATE TABLE [dbo].[TaskPriorities] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [DisplayOrder] INT            NOT NULL,
    [Name]         NVARCHAR (20) NULL
);