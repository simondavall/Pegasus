CREATE PROCEDURE [dbo].[spComments_Add]
	@TaskId int = 0,
	@Comment nvarchar(max),
	@UserId nvarchar(450)
AS
	INSERT INTO [dbo].[TaskComments]
           ([TaskId], [Comment], [IsDeleted], [UserId], [Modified], [Created])
	VALUES
           (@TaskId, @Comment, 0, @UserId, GETUTCDATE(), GETUTCDATE());

RETURN 0
