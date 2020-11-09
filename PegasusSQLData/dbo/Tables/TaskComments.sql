CREATE TABLE [dbo].[TaskComments] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [TaskId]    INT            NOT NULL,
    [Comment]   NVARCHAR (MAX) NULL,
    [IsDeleted] BIT            NOT NULL,
    [UserId]    NVARCHAR(450)  NULL,
    [Modified]  DATETIME2 (7)  NULL,
    [Created]   DATETIME2 (7)  NOT NULL,
);

