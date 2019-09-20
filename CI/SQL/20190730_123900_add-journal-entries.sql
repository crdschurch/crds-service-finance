USE [MinistryPlatform]
GO

-- ===============================================================
-- Authors: John Cleaver <john.cleaver@ingagepartners.com>
-- Create date: 7/30/2019
-- Description:	Journal Entries table to hold export data created from Distribution Adjustments
-- ===============================================================

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'cr_Journal_Entries')
BEGIN
  CREATE TABLE [dbo].cr_Journal_Entries(
	[Journal_Entry_ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Created_Date] DATETIME NOT NULL,
	[Exported_Date] DATETIME NULL,
	[GL_Account_Number] NVARCHAR(50) NOT NULL,
	[Batch_ID] NVARCHAR(15) NOT NULL,
	[Credit_Amount] MONEY NOT NULL,
	[Debit_Amount] MONEY NOT NULL,
	[Description] NVARCHAR(500),
	[Adjustment_Year] INT NOT NULL,
	[Adjustment_Month] INT NOT NULL,
	[Domain_ID] INT NOT NULL CONSTRAINT [DF_cr_Journal_Entries_Domain_ID] DEFAULT ((1))
  )
END
GO

