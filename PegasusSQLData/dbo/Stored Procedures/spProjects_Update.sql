CREATE PROCEDURE [dbo].[spProjects_Update]
	@id int = 0
	,@name nvarchar(50)
	,@projectPrefix nvarchar(20)

AS
	UPDATE [dbo].[Projects]
	SET [Name] = @name
		,[ProjectPrefix] = @projectPrefix
	WHERE Id = @id
RETURN 0



