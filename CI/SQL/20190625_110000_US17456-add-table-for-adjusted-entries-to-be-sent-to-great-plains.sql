USE [MinistryPlatform]
GO

-- ===============================================================
-- Authors: John Cleaver <john.cleaver@ingagepartners.com>
-- Create date: 6/25/2019
-- Description:	Journal Entries table to allow staging of data to external vendor
-- ===============================================================

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'cr_Adjusting_Journal_Entries')
BEGIN
  CREATE TABLE [dbo].cr_Adjusting_Journal_Entries(
    [Journal_Entry_ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Created_Date] DATETIME NOT NULL,
	[Donation_Date] DATETIME NOT NULL,
    [Sent_To_GL_Date] DATETIME NULL,
	[GL_Account_Number] NVARCHAR(20) NOT NULL,
    [Amount] MONEY NOT NULL,
    [Adjustment] NVARCHAR(75) NOT NULL,
    [Description] NVARCHAR(500),
	[Donation_Distribution_ID] INT FOREIGN KEY REFERENCES [Donation_Distributions]([Donation_Distribution_ID]),
    [Domain_ID] INT NOT NULL CONSTRAINT [DF_cr_Adjusting_Journal_Entries_Domain_ID] DEFAULT ((1))
  )
END
GO

