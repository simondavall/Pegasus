CREATE PROCEDURE [dbo].[spTasks_Get]
	@id int = 0
AS
	SELECT [Id], [Created], [Description], [Modified], [Name], [ProjectId], [TaskRef], [TaskStatusId], [TaskTypeId], [FixedInRelease], [TaskPriorityId]
	FROM ProjectTasks
	WHERE [Id] = @id

RETURN 0
