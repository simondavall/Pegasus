CREATE PROCEDURE [dbo].[spUsers_Update]
	@Id nvarchar(450)
	, @DisplayName nvarchar(256)
AS

IF EXISTS (SELECT [Id] FROM [dbo].[Users] WHERE [Id] = @Id)
BEGIN

	UPDATE  [dbo].[Users]
	SET [DisplayName] = @DisplayName
	WHERE [Id] = @Id

END
ELSE
BEGIN

	INSERT [dbo].[Users] (Id, DisplayName) VALUES (@Id, @DisplayName)

END

RETURN 0
