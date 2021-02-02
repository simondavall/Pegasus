CREATE TABLE [dbo].[ProjectTasks] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [TaskRef]        NVARCHAR (20)  NULL,
    [Name]           NVARCHAR (200) NULL,
    [Description]    NVARCHAR (MAX) NULL,
    [TaskStatusId]   INT            NOT NULL,
    [TaskTypeId]     INT            NOT NULL,
    [TaskPriorityId] INT            NOT NULL,
    [FixedInRelease] NVARCHAR (20)  NULL,
    [ProjectId]      INT            NOT NULL,
    [ParentTaskId]   INT            NULL,
    [UserId]         NVARCHAR(450)  NULL,
    [Modified]       DATETIME2 (7)  NOT NULL,
    [Created]        DATETIME2 (7)  NOT NULL
)
