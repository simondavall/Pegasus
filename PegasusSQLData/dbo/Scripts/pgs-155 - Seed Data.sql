/* Seed [TaskPriorities] */
SET IDENTITY_INSERT [Pegasus].[dbo].[TaskPriorities] ON;

if NOT EXISTS(select Id from [Pegasus].[dbo].[TaskPriorities])
begin
	INSERT INTO [Pegasus].[dbo].[TaskPriorities] ([Id], [Name], [DisplayOrder]) VALUES (1, 'None', 4)
	INSERT INTO [Pegasus].[dbo].[TaskPriorities] ([Id], [Name], [DisplayOrder]) VALUES (2, 'Low', 3)
	INSERT INTO [Pegasus].[dbo].[TaskPriorities] ([Id], [Name], [DisplayOrder]) VALUES (3, 'Normal', 0)
	INSERT INTO [Pegasus].[dbo].[TaskPriorities] ([Id], [Name], [DisplayOrder]) VALUES (4, 'High', 2)
	INSERT INTO [Pegasus].[dbo].[TaskPriorities] ([Id], [Name], [DisplayOrder]) VALUES (5, 'Critical', 1)
end

SET IDENTITY_INSERT [Pegasus].[dbo].[TaskPriorities] OFF;

/* Seed [TaskStatus] */
SET IDENTITY_INSERT [Pegasus].[dbo].[TaskStatus] ON;

if NOT EXISTS(select Id from [Pegasus].[dbo].[TaskStatus])
begin
	INSERT INTO [Pegasus].[dbo].[TaskStatus] ([Id], [Name], [DisplayOrder]) VALUES (1, 'Submitted', 0)
	INSERT INTO [Pegasus].[dbo].[TaskStatus] ([Id], [Name], [DisplayOrder]) VALUES (2, 'In Progress', 10)
	INSERT INTO [Pegasus].[dbo].[TaskStatus] ([Id], [Name], [DisplayOrder]) VALUES (3, 'Completed', 99)
	INSERT INTO [Pegasus].[dbo].[TaskStatus] ([Id], [Name], [DisplayOrder]) VALUES (4, 'Backlog', 30)
	INSERT INTO [Pegasus].[dbo].[TaskStatus] ([Id], [Name], [DisplayOrder]) VALUES (5, 'Obsolete', 80)
	INSERT INTO [Pegasus].[dbo].[TaskStatus] ([Id], [Name], [DisplayOrder]) VALUES (6, 'On Hold', 20)
end

SET IDENTITY_INSERT [Pegasus].[dbo].[TaskStatus] OFF;

/* Seed [TaskTypes] */
SET IDENTITY_INSERT [Pegasus].[dbo].[TaskTypes] ON;

if NOT EXISTS(select Id from [Pegasus].[dbo].[TaskTypes])
begin
	INSERT INTO [Pegasus].[dbo].[TaskTypes] ([Id], [Name], [DisplayOrder]) VALUES (1, 'Task', 0)
	INSERT INTO [Pegasus].[dbo].[TaskTypes] ([Id], [Name], [DisplayOrder]) VALUES (2, 'Bug', 10)
end

SET IDENTITY_INSERT [Pegasus].[dbo].[TaskTypes] OFF;
