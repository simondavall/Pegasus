CREATE PROCEDURE [dbo].[spTasks_GetAllTaskTypes]

AS
	SELECT [Id], [DisplayOrder], [Name]
	FROM TaskTypes
	ORDER BY DisplayOrder

RETURN 0
