﻿CREATE PROCEDURE [dbo].[spTasks_Add]
	@Name nvarchar(200)
	, @Description nvarchar(max)
	, @ProjectId int
	, @TaskStatusId int
	, @TaskTypeId int
	, @TaskPriorityId int
	, @FixedInRelease nvarchar(20)
	, @UserId nvarchar(450)
	, @ParentTaskId int = NULL

AS

declare @TaskRef nvarchar(20)
declare @TaskId int = 0

BEGIN TRANSACTION;

BEGIN TRY

-- Create the TaskRef from next task index 
	SELECT @TaskRef = [ProjectPrefix] + '-' + CONVERT(nvarchar(10), [NextIndex])
	FROM [dbo].[Projects] p
		join [dbo].[TaskIndexers] t on t.ProjectId = p.Id
	WHERE p.Id = @ProjectId

-- Increment the NextIndex value in task indexer
	UPDATE [dbo].[TaskIndexers]
	SET NextIndex = NextIndex + 1
	WHERE ProjectId = @ProjectId

-- Insert new project record
	INSERT INTO [dbo].[ProjectTasks]
           ([TaskRef], [Name], [Description], [ProjectId], [ParentTaskId], [TaskStatusId], [TaskTypeId], [TaskPriorityId], [FixedInRelease], [UserId], [Created], [Modified])
     VALUES
           (@TaskRef, @Name, @Description, @ProjectId, @ParentTaskId, @TaskStatusId, @TaskTypeId, @TaskPriorityId, @FixedInRelease, @UserId, GETUTCDATE(), GETUTCDATE())

	SET @TaskId = @@IDENTITY

-- Insert new entry into status history
	INSERT INTO [dbo].[StatusHistory]
			([TaskId], [TaskStatusId], [UserId], [Created])
	VALUES
			(@TaskId, @TaskStatusId, @UserId, GETUTCDATE())

END TRY
BEGIN CATCH
	IF @@TRANCOUNT > 0
		ROLLBACK TRANSACTION
	SET @TaskId = 0
END CATCH

IF @@TRANCOUNT > 0
	COMMIT TRANSACTION


SELECT @TaskId