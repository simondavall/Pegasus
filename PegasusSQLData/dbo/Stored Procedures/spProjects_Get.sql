CREATE PROCEDURE [dbo].[spProjects_Get]
	@id int = 0
AS
	SET NOCOUNT ON

	SELECT [Id], [Name], [ProjectPrefix], [IsPinned], [IsActive]
	FROM [dbo].[Projects]
	WHERE [Id] = @id
RETURN 0
