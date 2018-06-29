-- =============================================
-- Author:      John Cleaver
-- Create date: 2018-2-8
-- Description:	Add Vendor_Detail_Url column
--              This was originally in crds-angular pushpay branch
-- =============================================

USE MinistryPlatform
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
            WHERE Name = N'Vendor_Detail_Url' AND Object_ID = Object_ID(N'dbo.Recurring_Gifts'))
BEGIN
	ALTER TABLE dbo.Recurring_Gifts ADD Vendor_Detail_Url NVARCHAR(2000)
END
GO

-- Update MyHouseholdDonationRecurringGifts page
DECLARE @MyHouseholdDonationRecurringGifts_Page_ID AS INT = 523

UPDATE dbo.dp_Pages
	SET 
		[Default_Field_List] = CONCAT([Default_Field_List], ', Recurring_Gifts.[Vendor_Detail_Url]')
	WHERE
		Page_ID = @MyHouseholdDonationRecurringGifts_Page_ID 
GO