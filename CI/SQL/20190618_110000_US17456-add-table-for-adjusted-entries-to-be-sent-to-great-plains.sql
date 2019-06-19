USE [MinistryPlatform]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===============================================================
-- Authors: John Cleaver <john.cleaver@ingagepartners.com>
-- Create date: 6/18/2019
-- Description:	Journal Entries table to allow staging of data to external vendor
-- ===============================================================

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE table_name='cr_Journal_Entries')
BEGIN
  CREATE TABLE [dbo].cr_Journal_Entries(
    [Journal_Entry_ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [GL_Account_Mapping_ID] INT FOREIGN KEY REFERENCES [GL_Account_Mapping]([GL_Account_Mapping_ID]),
    [Created_Date] DATETIME NOT NULL,
    [For_Date] DATETIME NOT NULL,
    [Account] NVARCHAR(15) NOT NULL,
    [Adjustment] MONEY NOT NULL,
    [Description] NVARCHAR(500) NOT NULL,
    [Domain_ID] INT NOT NULL CONSTRAINT [DF_cr_Journal_Entries_Domain_ID] DEFAULT ((1))
  )
END
GO

