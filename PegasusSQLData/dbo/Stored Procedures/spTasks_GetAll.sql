CREATE PROCEDURE [dbo].[spTasks_GetAll]

AS
	SELECT [Id], [TaskRef], [Name], [Description], [TaskStatusId], [TaskTypeId], [TaskPriorityId], [FixedInRelease], [ProjectId], [UserId], [Modified], [Created]
	FROM ProjectTasks
	ORDER BY Modified DESC

RETURN 0
