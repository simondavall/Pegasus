CREATE PROCEDURE [dbo].[spProjects_GetAll]

AS
	SET NOCOUNT ON

	SELECT [Id], [Name], [ProjectPrefix]
	FROM [dbo].[Projects]
	ORDER BY [Id]
RETURN 0


