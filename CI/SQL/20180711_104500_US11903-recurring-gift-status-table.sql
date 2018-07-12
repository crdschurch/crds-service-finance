-- =============================================
-- Author:      Brian Cavanaugh
-- Create date: 2018-07-11
-- Description:	Create cr_Recurring_Gift_Status table 
-- and add foreign key to it in Recurring_Gifts table
-- =============================================
USE MinistryPlatform
GO

-- Create cr_Recurring_Gift_Status table
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' 
	and TABLE_NAME = 'cr_Recurring_Gift_Status')
BEGIN
    CREATE TABLE dbo.cr_Recurring_Gift_Status
    (
		Recurring_Gift_Status_ID int IDENTITY(1, 1) not null,
        Recurring_Gift_Status varchar(20) not null,
		CONSTRAINT PK_Recurring_Gift_Status_ID PRIMARY KEY CLUSTERED (Recurring_Gift_Status_ID)
    )    
END
GO

-- Seed new cr_Recurring_Gift_Status table with initial values
SET IDENTITY_INSERT cr_Recurring_Gift_Status ON

IF NOT EXISTS (SELECT 1 FROM cr_Recurring_Gift_Status WHERE Recurring_Gift_Status = 'Active')
BEGIN
INSERT INTO [dbo].[cr_Recurring_Gift_Status]
           ([Recurring_Gift_Status_ID]
		   ,[Recurring_Gift_Status])
     VALUES
           (1
		   ,'Active')
END

IF NOT EXISTS (SELECT 1 FROM cr_Recurring_Gift_Status WHERE Recurring_Gift_Status = 'Paused')
BEGIN
INSERT INTO [dbo].[cr_Recurring_Gift_Status]
           ([Recurring_Gift_Status_ID]
		   ,[Recurring_Gift_Status])
     VALUES
           (2
		   ,'Paused')
END

IF NOT EXISTS (SELECT 1 FROM cr_Recurring_Gift_Status WHERE Recurring_Gift_Status = 'Cancelled')
BEGIN
INSERT INTO [dbo].[cr_Recurring_Gift_Status]
           ([Recurring_Gift_Status_ID]
		   ,[Recurring_Gift_Status])
     VALUES
           (3
		   ,'Cancelled')
END

SET IDENTITY_INSERT cr_Recurring_Gift_Status OFF

GO

-- Alter Recurring_Gifts table to add new Recurring_Gift_Status_ID field
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' 
	and TABLE_NAME = 'Recurring_Gifts' 
	and COLUMN_NAME = 'Recurring_Gift_Status_ID')
BEGIN
ALTER TABLE dbo.Recurring_Gifts 
	ADD Recurring_Gift_Status_ID int NULL;
END
GO

-- Alter Recurring_Gifts table to add new foreign key back to Recurring_Gifts_Status table
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA = 'dbo' 
	and TABLE_NAME = 'Recurring_Gifts' 
	and CONSTRAINT_NAME = 'FK_Recurring_Gift_Recurring_Gifts_Status')
BEGIN
ALTER TABLE dbo.Recurring_Gifts
	ADD CONSTRAINT FK_Recurring_Gifts_Recurring_Gift_Status FOREIGN KEY ([Recurring_Gift_Status_ID])
		REFERENCES dbo.cr_Recurring_Gift_Status ([Recurring_Gift_Status_ID]);
END
GO

-- Add record to dp_Pages table to allow proper display in MP
IF NOT EXISTS (SELECT 1 FROM dp_Pages WHERE Display_Name = 'Recurring Gift Statuses')
BEGIN
INSERT INTO [dbo].[dp_Pages]
           ([Display_Name]
           ,[Singular_Name]
           ,[Description]
           ,[View_Order]
           ,[Table_Name]
           ,[Primary_Key]
           ,[Default_Field_List]
           ,[Selected_Record_Expression]
           ,[Display_Copy])
     VALUES
           ('Recurring Gift Statuses'
           ,'Recurring Gift Status'
           ,'Recurring gift statuses for Recurring Gifts'
           ,100
           ,'cr_Recurring_Gift_Status'
           ,'Recurring_Gift_Status_ID'
           ,'Recurring_Gift_Status'
           ,'Recurring_Gift_Status'
           ,0)
END
GO

-- Set default values for Recurring Gift Status ID
UPDATE Recurring_Gifts
SET Recurring_Gift_Status_ID = 1
WHERE End_Date is null

UPDATE Recurring_Gifts
SET Recurring_Gift_Status_ID = 3
WHERE End_Date is not null
