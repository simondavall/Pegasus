CREATE PROCEDURE [dbo].[spTasks_GetStatusHistory]
	@TaskId int = 0
AS
	SELECT [Id], [TaskId], [TaskStatusId], [UserId], [Created]
	FROM [dbo].[StatusHistory]
	WHERE [TaskId] = @TaskId
	ORDER BY [Created]

RETURN 0
