CREATE PROCEDURE [dbo].[spTasks_GetAll]

AS
	SELECT pt.*
	FROM ProjectTasks pt JOIN Projects p ON pt.ProjectId = p.Id
	WHERE ParentTaskId is null AND p.IsDeleted = 0 AND p.IsActive = 1
	ORDER BY Modified DESC

RETURN 0
