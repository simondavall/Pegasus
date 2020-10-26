CREATE TABLE [dbo].[TaskComments] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Comment]   NVARCHAR (MAX) NULL,
    [Created]   DATETIME2 (7)  NOT NULL,
    [TaskId]    INT            NOT NULL,
    [IsDeleted] BIT            NOT NULL
);

