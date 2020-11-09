CREATE PROCEDURE [dbo].[spTasks_GetAllForProject]
	@projectId int = 0
AS
	SELECT [Id], [TaskRef], [Name], [Description], [TaskStatusId], [TaskTypeId], [TaskPriorityId], [FixedInRelease], [ProjectId], [UserId], [Modified], [Created]
	FROM ProjectTasks
	WHERE ProjectId = @projectId
	ORDER BY Modified DESC

RETURN 0
