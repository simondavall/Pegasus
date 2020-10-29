CREATE PROCEDURE [dbo].[spProjects_Delete]
	@id int = 0
AS
		
DELETE FROM [dbo].[ProjectTasks]
            WHERE ProjectId = @id

DELETE FROM [dbo].[Projects]
			WHERE Id = @id

RETURN 0
