CREATE PROCEDURE [dbo].[spProjects_GetAll]

AS
	SET NOCOUNT ON

	SELECT [Id], [Name], [ProjectPrefix], [IsPinned], [IsActive]
	FROM [dbo].[Projects]
	WHERE [IsDeleted] = 0 
	ORDER BY [Id]
RETURN 0


