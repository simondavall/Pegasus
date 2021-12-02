CREATE PROCEDURE [dbo].[spTasks_GetAllForProject]
	@projectId int = 0
AS
	SELECT pt.*, count(pt2.Id) as SubTaskCount
	FROM ProjectTasks pt
	LEFT OUTER JOIN ProjectTasks pt2 on pt2.ParentTaskId = pt.Id
	WHERE pt.ProjectId = @projectId AND pt.ParentTaskId is null
	GROUP BY pt.Id, pt.[Id]
      ,pt.[TaskRef]
      ,pt.[Name]
      ,pt.[Description]
      ,pt.[TaskStatusId]
      ,pt.[TaskTypeId]
      ,pt.[TaskPriorityId]
      ,pt.[FixedInRelease]
      ,pt.[ProjectId]
      ,pt.[ParentTaskId]
      ,pt.[UserId]
      ,pt.[Modified]
      ,pt.[Created]
	ORDER BY pt.Modified DESC

RETURN 0
