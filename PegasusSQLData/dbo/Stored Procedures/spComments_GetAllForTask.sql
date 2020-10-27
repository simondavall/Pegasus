CREATE PROCEDURE [dbo].[spComments_GetAllForTask]
	@TaskId int = 0
AS
	SELECT [Id], [Comment], [Created], [TaskId], [IsDeleted] 
	FROM [dbo].[TaskComments]
	WHERE [TaskId] = @TaskId AND [IsDeleted] = 0
	ORDER BY [Created]

RETURN 0
