CREATE PROCEDURE [dbo].[spTasks_GetAllTaskPriorities]

AS
	SELECT [Id], [DisplayOrder], [Name]
	FROM TaskPriorities
	ORDER BY DisplayOrder

RETURN 0
