CREATE PROCEDURE [dbo].[spProjects_Update]
	@id int = 0
	,@name nvarchar(50)
	,@projectPrefix nvarchar(20)
	,@isPinned bit
	,@isActive bit

AS
	UPDATE [dbo].[Projects]
	SET [Name] = @name
		,[ProjectPrefix] = @projectPrefix
		,[IsPinned] = @isPinned
		,[IsActive] = @isActive
	WHERE Id = @id
RETURN 0



