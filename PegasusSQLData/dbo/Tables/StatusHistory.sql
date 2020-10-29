CREATE TABLE [dbo].[StatusHistory] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [Created]      DATETIME2 (7) NOT NULL,
    [TaskId]       INT           NOT NULL,
    [TaskStatusId] INT           NOT NULL 
)
