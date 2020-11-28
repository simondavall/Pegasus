/*
Manual deployment script

This should be run during deployment of branch pgs-78

The following scripts populate the new UserId and Modified fields
and add initial data to the new Users table

Change the declared variables to correct values for Production and Execute.

*/

declare @UserId nvarchar(450) = 'f7d69a12-ab97-4cf4-916a-da96f403a0e7'
declare @DisplayName nvarchar(256) = 'Simon Da Vall'


update TaskComments
Set Modified = Created
where Modified IS NULL

Update ProjectTasks
Set UserId = @UserId
where UserId IS NULL

Update TaskComments
Set UserId = @UserId
where UserId IS NULL

Update StatusHistory
Set UserId = @UserId
where UserId IS NULL

Insert Users (Id, DisplayName)
Values (@UserId, @DisplayName)
