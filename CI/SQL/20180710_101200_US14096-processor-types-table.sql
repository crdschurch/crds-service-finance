-- =============================================
-- Author:      Brian Cavanaugh
-- Create date: 2018-07-10
-- Description:	Create cr_Processor_Types table 
-- and add foreign key to it in Donor_Accounts table
-- =============================================
USE MinistryPlatform
GO

-- drop table dbo.cr_Processor_Types
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' 
	and TABLE_NAME = 'cr_Processor_Types')
BEGIN
    CREATE TABLE dbo.cr_Processor_Types
    (
		Processor_Type_ID int IDENTITY(1, 1) not null,
        Processor_Type varchar(20) not null,
		CONSTRAINT PK_Processor_Types_ID PRIMARY KEY CLUSTERED (Processor_Type_ID)
    )    
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' 
	and TABLE_NAME = 'Donor_Accounts' 
	and COLUMN_NAME = 'Processor_Type_ID')
BEGIN
ALTER TABLE dbo.Donor_Accounts 
	ADD Processor_Type_ID int NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA = 'dbo' 
	and TABLE_NAME = 'Donor_Accounts' 
	and CONSTRAINT_NAME = 'FK_Donor_Accounts_Processor_Types')
BEGIN
ALTER TABLE dbo.Donor_Accounts 
	ADD CONSTRAINT FK_Donor_Accounts_Processor_Types FOREIGN KEY (Processor_Type_ID)
		REFERENCES dbo.cr_Processor_Types (Processor_Type_ID);
END
GO

IF NOT EXISTS (SELECT 1 FROM dp_Pages WHERE Display_Name = 'Processor Types')
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
           ('Processor Types'
           ,'Processor Type'
           ,'Processor types for Donor Accounts'
           ,100
           ,'cr_Processor_Types'
           ,'Processor_Type_ID'
           ,'Processor_Type'
           ,'Processor_Type'
           ,0)
END
GO

IF NOT EXISTS (SELECT 1 FROM cr_Processor_Types WHERE Processor_Type = 'Pushpay')
BEGIN

SET IDENTITY_INSERT cr_Processor_Types ON

INSERT INTO [dbo].[cr_Processor_Types]
           ([Processor_Type_ID]
		   ,[Processor_Type])
     VALUES
           (1
		   ,'Pushpay')

SET IDENTITY_INSERT cr_Processor_Types OFF

END
GO
