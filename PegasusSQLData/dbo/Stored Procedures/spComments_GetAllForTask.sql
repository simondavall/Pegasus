CREATE PROCEDURE [dbo].[spComments_GetAllForTask]
	@TaskId int = 0
AS
	SELECT c.[Id], [TaskId], [Comment], [IsDeleted], [UserId], [UserName], [Modified], [Created]
	FROM [dbo].[TaskComments] c
	LEFT OUTER JOIN [dbo].Users u ON u.Id = c.UserId
	WHERE [TaskId] = @TaskId AND [IsDeleted] = 0
	ORDER BY [Created]

RETURN 0
