CREATE PROCEDURE [dbo].[spTasks_GetByRef]
	@taskRef varchar(20) = ''
AS
	SELECT *
	FROM ProjectTasks
	WHERE [TaskRef] = @taskRef
RETURN 0
