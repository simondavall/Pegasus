CREATE PROCEDURE [dbo].[spComments_Update]
	@Id int = 0,
	@Comment nvarchar(max),
	@IsDeleted bit = 0
AS
	UPDATE [dbo].[TaskComments]
	SET [Comment] = @Comment ,[IsDeleted] = @IsDeleted
	WHERE Id = @Id

RETURN 0
