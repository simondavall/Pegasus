CREATE PROCEDURE [dbo].[spTasks_AddTaskType]
	@Name nvarchar(20)
	, @DisplayOrder int
AS
	IF NOT EXISTS(SELECT Id FROM  [dbo].[TaskTypes] WHERE [Name]= @Name)
	INSERT INTO [dbo].[TaskTypes]
			([Name], [DisplayOrder])
	VALUES
			(@Name, @DisplayOrder)

RETURN 0
