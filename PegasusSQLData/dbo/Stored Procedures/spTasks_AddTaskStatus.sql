CREATE PROCEDURE [dbo].[spTasks_AddTaskStatus]
	@Name nvarchar(20)
	, @DisplayOrder int
AS

IF NOT EXISTS(SELECT Id FROM  [dbo].[TaskStatus]  WHERE [Name]= @Name)
	INSERT INTO [dbo].[TaskStatus]
			([Name], [DisplayOrder])
	VALUES
			(@Name, @DisplayOrder)

RETURN 0
