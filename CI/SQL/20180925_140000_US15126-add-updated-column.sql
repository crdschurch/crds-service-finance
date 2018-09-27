USE MinistryPlatform
GO

-- =============================================
-- Author:      John Cleaver
-- Create date: 2018-09-25
-- Description:	Add updated on column to recurring gifts
-- =============================================

IF NOT EXISTS(SELECT * FROM sys.columns 
            WHERE Name = N'Updated_On' AND Object_ID = Object_ID(N'dbo.[Recurring_Gifts]'))
BEGIN
	ALTER TABLE [dbo].[Recurring_Gifts] ADD Updated_On DATETIME NULL
END
GO