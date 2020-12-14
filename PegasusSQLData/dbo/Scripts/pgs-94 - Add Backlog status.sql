/*
Manual deployment script

This should be run during deployment of branch pgs-94

The following script adds a new status 'Backlog' to the TaskStatus table

Change the declared variables to correct values for Production and Execute.

*/


declare @StatusId int = 4
declare @StatusName nvarchar(20) = 'Backlog'
declare @DisplayOrder int = 30

IF NOT EXISTS (SELECT * FROM [dbo].[TaskStatus] WHERE [Id] = @StatusId)
BEGIN
    SET IDENTITY_INSERT TaskStatus ON

    Insert dbo.[TaskStatus]([Id], [Name], [DisplayOrder])
    Values (@StatusId, @StatusName, @DisplayOrder)

    SET IDENTITY_INSERT TaskStatus OFF
END
