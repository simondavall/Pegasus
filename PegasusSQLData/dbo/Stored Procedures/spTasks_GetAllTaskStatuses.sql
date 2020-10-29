CREATE PROCEDURE [dbo].[spTasks_GetAllTaskStatuses]

AS
	SELECT [Id], [DisplayOrder], [Name]
	FROM TaskStatus
	ORDER BY DisplayOrder

RETURN 0
