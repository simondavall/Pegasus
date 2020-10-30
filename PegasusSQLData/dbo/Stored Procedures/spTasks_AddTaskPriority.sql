CREATE PROCEDURE [dbo].[spTasks_AddTaskPriority]
	@Name nvarchar(20)
	, @DisplayOrder int
AS
	IF NOT EXISTS(SELECT Id FROM  [dbo].[TaskPriorities]  WHERE [Name]= @Name)
	INSERT INTO [dbo].[TaskPriorities]
			([Name], [DisplayOrder])
	VALUES
			(@Name, @DisplayOrder)

RETURN 0
