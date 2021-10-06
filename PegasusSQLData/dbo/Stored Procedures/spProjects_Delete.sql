CREATE PROCEDURE [dbo].[spProjects_Delete]
	@id int = 0
AS
		
	UPDATE [dbo].[Projects]
	SET [IsDeleted] = 1
	WHERE Id = @id
RETURN 0