CREATE PROCEDURE [dbo].[spTasks_Get]
	@id int = 0
AS
	SELECT [Id], [TaskRef], [Name], [Description], [TaskStatusId], [TaskTypeId], [TaskPriorityId], [FixedInRelease], [ProjectId], [UserId], [Modified], [Created]
	FROM ProjectTasks
	WHERE [Id] = @id

RETURN 0
