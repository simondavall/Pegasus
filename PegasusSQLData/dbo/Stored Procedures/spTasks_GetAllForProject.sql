CREATE PROCEDURE [dbo].[spTasks_GetAllForProject]
	@projectId int = 0
AS
	SELECT *
	FROM ProjectTasks
	WHERE ProjectId = @projectId AND ParentTaskId is null
	ORDER BY Modified DESC

RETURN 0
