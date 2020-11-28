CREATE PROCEDURE [dbo].[spUsers_GetById]
	@UserId nvarchar(450)
AS
	SELECT [Id], [DisplayName] FROM Users WHERE [Id] = @UserId
RETURN 0
