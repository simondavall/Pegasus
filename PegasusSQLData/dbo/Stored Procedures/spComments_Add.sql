CREATE PROCEDURE [dbo].[spComments_Add]
	@TaskId int = 0,
	@Comment nvarchar(max)
AS
	INSERT INTO [dbo].[TaskComments]
           ([TaskId], [Comment], [Created], [IsDeleted])
	VALUES
           (@TaskId, @Comment, GETUTCDATE(), 0);

RETURN 0
