CREATE PROCEDURE [dbo].[spTasks_GetSubTasks]
	@taskId int = 0
AS
	SELECT *
	FROM ProjectTasks
	WHERE ParentTaskId = @taskId
	ORDER BY Modified DESC

RETURN 0
