CREATE PROCEDURE [dbo].[spTasks_GetAllForProject]
	@projectId int = 0
AS
	SELECT [Id], [Created], [Description], [Modified], [Name], [ProjectId], [TaskRef], [TaskStatusId], [TaskTypeId], [FixedInRelease], [TaskPriorityId]
	FROM ProjectTasks
	WHERE ProjectId = @projectId
	ORDER BY Modified DESC

RETURN 0
