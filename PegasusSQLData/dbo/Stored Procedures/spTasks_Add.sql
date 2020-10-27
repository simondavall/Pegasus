CREATE PROCEDURE [dbo].[spTasks_Add]
	@Name nvarchar(200)
	, @Description nvarchar(max)
	, @ProjectId int
	, @TaskStatusId int
	, @TaskTypeId int
	, @TaskPriorityId int
	, @FixedInRelease nvarchar(20)

AS

declare @TaskRef nvarchar(20)

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
           ([TaskRef], [Name], [Description], [ProjectId], [TaskStatusId], [TaskTypeId], [TaskPriorityId], [FixedInRelease], [Created], [Modified])
     VALUES
           (@TaskRef, @Name, @Description, @ProjectId, @TaskStatusId, @TaskTypeId, @TaskPriorityId, @FixedInRelease, GETUTCDATE(), GETUTCDATE())

	declare @TaskId int = @@IDENTITY

-- Insert new entry into status history
	INSERT INTO [dbo].[StatusHistory]
			([TaskId], [TaskStatusId], [Created])
	VALUES
			(@TaskId, @TaskStatusId, GETUTCDATE())

END TRY
BEGIN CATCH
	IF @@TRANCOUNT > 0
		ROLLBACK TRANSACTION
		
END CATCH

IF @@TRANCOUNT > 0
	COMMIT TRANSACTION

RETURN 0
