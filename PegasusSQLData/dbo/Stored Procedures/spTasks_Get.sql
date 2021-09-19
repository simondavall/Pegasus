CREATE PROCEDURE [dbo].[spTasks_Get]
	@id int = 0
AS
	SELECT *
	FROM ProjectTasks
	WHERE [Id] = @id

RETURN 0
