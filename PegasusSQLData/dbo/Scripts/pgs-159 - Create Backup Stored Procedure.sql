USE [master]
GO

/****** Object:  StoredProcedure [dbo].[sp_BackupSingleDatabase]    Script Date: 29/10/2021 00:31:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_BackupSingleDatabase]   
            @databaseName sysname = null, 
			@backupFileName nvarchar(128),
            @backupLocation nvarchar(200)  
AS  
       SET NOCOUNT ON;  
            DECLARE @DBs TABLE 
            ( 
                  ID int IDENTITY PRIMARY KEY, 
                  DBNAME nvarchar(500) 
            ) 

            -- Declare variables 
            DECLARE @BackupName nvarchar(100) 
            DECLARE @BackupFile nvarchar(300) 
            DECLARE @DBNAME nvarchar(300) 
            DECLARE @sqlCommand NVARCHAR(1000)  

		SET @DBNAME = '['+@databaseName+']' 
		SET @BackupFile = @backupLocation + @backupFileName
		SET @BackupName = REPLACE(REPLACE(@DBNAME,'[',''),']','') +' full backup'
		SET @sqlCommand = 'BACKUP DATABASE ' +@DBNAME+  ' TO DISK = '''+@BackupFile+ ''' WITH INIT, NAME= ''' +@BackupName+''', NOSKIP, NOFORMAT' 
 
		EXEC(@sqlCommand) 

GO
