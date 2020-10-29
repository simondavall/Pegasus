CREATE TABLE [dbo].[TaskIndexers] (
    [Id]        INT IDENTITY (1, 1) NOT NULL,
    [NextIndex] INT NOT NULL,
    [ProjectId] INT NOT NULL
);