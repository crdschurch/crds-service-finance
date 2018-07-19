-- =============================================
-- Author:      Tim Giblin
-- Create date: 2018-7-19
-- Description:	Add Vendor_Detail_Url column to hold pushpay link to deposit
-- =============================================

USE MinistryPlatform
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
            WHERE Name = N'Vendor_Detail_Url' AND Object_ID = Object_ID(N'dbo.Deposits'))
BEGIN
	ALTER TABLE dbo.Deposits ADD Vendor_Detail_Url VARCHAR(300)
END
GO

-- Update Deposits page
DECLARE @Deposits_Page_ID AS INT = 294

UPDATE dbo.dp_Pages
	SET 
		[Default_Field_List] = CONCAT([Default_Field_List], ', Deposits.[Vendor_Detail_Url]')
	WHERE
		Page_ID = @Deposits_Page_ID 
GO