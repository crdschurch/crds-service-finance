USE MinistryPlatform
GO

-- =============================================
-- Author:      Tim Giblin
-- Create date: 2018-09-26
-- Description:	Add admin url and notes field to recurring gifts
-- =============================================

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'Vendor_Admin_Detail_Url' AND Object_ID = Object_ID(N'dbo.[Recurring_Gifts]'))
BEGIN
	ALTER TABLE [dbo].[Recurring_Gifts] ADD Vendor_Admin_Detail_Url NVARCHAR(2000) NULL
END
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'Notes' AND Object_ID = Object_ID(N'dbo.[Recurring_Gifts]'))
BEGIN
    ALTER TABLE [dbo].[Recurring_Gifts] ADD Notes NVARCHAR(500) NULL
END
GO

-- Update MyHouseholdDonationRecurringGifts page
DECLARE @MyHouseholdDonationRecurringGifts_Page_ID AS INT = 523

UPDATE dbo.dp_Pages
	SET 
		[Default_Field_List] = CONCAT([Default_Field_List], ', Recurring_Gifts.[Vendor_Admin_Detail_Url], Recurring_Gifts.[Notes]')
	WHERE
		Page_ID = @MyHouseholdDonationRecurringGifts_Page_ID
GO