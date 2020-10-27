CREATE PROCEDURE [dbo].[spTasks_GetAll]

AS
	SELECT [Id], [Created], [Description], [Modified], [Name], [ProjectId], [TaskRef], [TaskStatusId], [TaskTypeId], [FixedInRelease], [TaskPriorityId]
	FROM ProjectTasks
	ORDER BY Modified DESC

RETURN 0
