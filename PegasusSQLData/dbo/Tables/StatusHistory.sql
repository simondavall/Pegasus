CREATE TABLE [dbo].[StatusHistory] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [TaskId]       INT           NOT NULL,
    [TaskStatusId] INT           NOT NULL,
    [UserId]       NVARCHAR(450) NULL,
    [Created]      DATETIME2 (7) NOT NULL,
)
