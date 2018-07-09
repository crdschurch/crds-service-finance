-- =============================================
-- Author:      Brian Cavanaugh
-- Create date: 2018-07-09
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
		Processor_Type_Id int IDENTITY(1, 1) not null,
        Processor_Type varchar(20) not null,
		CONSTRAINT PK_Processor_Types_Id PRIMARY KEY CLUSTERED (Processor_Type_Id)
    )    
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' 
	and TABLE_NAME = 'Donor_Accounts' 
	and COLUMN_NAME = 'Processor_Type_Id')
BEGIN
ALTER TABLE dbo.Donor_Accounts 
	ADD Processor_Type_Id int NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA = 'dbo' 
	and TABLE_NAME = 'Donor_Accounts' 
	and CONSTRAINT_NAME = 'FK_Donor_Accounts_Processor_Types')
BEGIN
ALTER TABLE dbo.Donor_Accounts 
	ADD CONSTRAINT FK_Donor_Accounts_Processor_Types FOREIGN KEY (Processor_Type_Id)
		REFERENCES dbo.cr_Processor_Types (Processor_Type_Id);
END
GO
