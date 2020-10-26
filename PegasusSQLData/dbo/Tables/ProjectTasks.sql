CREATE TABLE [dbo].[ProjectTasks] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [Created]        DATETIME2 (7)  NOT NULL,
    [Description]    NVARCHAR (MAX) NULL,
    [Modified]       DATETIME2 (7)  NOT NULL,
    [Name]           NVARCHAR (200) NULL,
    [ProjectId]      INT            NOT NULL,
    [TaskRef]        NVARCHAR (20) NULL,
    [TaskStatusId]   INT            NOT NULL,
    [TaskTypeId]     INT            NOT NULL,
    [FixedInRelease] NVARCHAR (20) NULL,
    [TaskPriorityId] INT            NOT NULL 
)
