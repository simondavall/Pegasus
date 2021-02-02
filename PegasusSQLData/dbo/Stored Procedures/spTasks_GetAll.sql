CREATE PROCEDURE [dbo].[spTasks_GetAll]

AS
	SELECT *
	FROM ProjectTasks
	WHERE ParentTaskId is null
	ORDER BY Modified DESC

RETURN 0
