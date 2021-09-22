CREATE PROCEDURE [dbo].[spProjects_Add]
	@Name nvarchar(50)
	, @ProjectPrefix nvarchar(20)
    , @IsPinned bit = 0
    , @IsActive bit = 1
AS
-- Insert new project record
	INSERT INTO [dbo].[Projects]
           ([Name]
           ,[ProjectPrefix]
           ,[IsPinned]
           ,[IsActive])
     VALUES
           (@Name, @ProjectPrefix, @IsPinned, @IsActive);

declare @ProjectId int = @@IDENTITY;

-- Add new entry into the TaskIndexer table for the new project
    INSERT INTO [dbo].[TaskIndexers]
        ([NextIndex], [ProjectId])
        VALUES
        (1, @ProjectId);

RETURN 0


