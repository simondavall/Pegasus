CREATE PROCEDURE [dbo].[spTasks_Update]
	@Id int = 0
	, @Name nvarchar(200)
	, @Description nvarchar(max)
	, @ProjectId int
	, @TaskStatusId int
	, @TaskTypeId int
	, @TaskPriorityId int
	, @FixedInRelease nvarchar(20)
	, @UserId nvarchar(450)
AS

BEGIN TRANSACTION

BEGIN TRY

-- Check whether status has changed, if so add new entry to StatusHistory
declare @currentStatus int
	SELECt @currentStatus = [TaskStatusId] FROM [ProjectTasks] WHERE [Id] = @Id

	IF @currentStatus <> @TaskStatusId
		INSERT [dbo].[StatusHistory] (TaskId, TaskStatusId, UserId, Created) VALUES (@Id, @TaskStatusId, @UserId, GETUTCDATE())

	UPDATE [dbo].[ProjectTasks]
	SET [Name] = @Name
		,[Description] = @Description
		,[TaskStatusId] = @TaskStatusId
		, [TaskTypeId] = @TaskTypeId
		, [TaskPriorityId] = @TaskPriorityId
		, [FixedInRelease] = @FixedInRelease
		, [Modified] = GETUTCDATE()
	WHERE Id = @Id

END TRY
BEGIN CATCH

	IF @@TRANCOUNT > 0
		ROLLBACK TRANSACTION

END CATCH

IF @@TRANCOUNT > 0
	COMMIT TRANSACTION

RETURN 0
