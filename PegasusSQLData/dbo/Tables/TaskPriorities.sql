CREATE TABLE [dbo].[TaskPriorities] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (20) NULL,
    [DisplayOrder] INT           NOT NULL
);