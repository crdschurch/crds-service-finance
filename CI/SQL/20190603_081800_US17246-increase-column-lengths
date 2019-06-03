USE [MinistryPlatform]
GO

-- =============================================
-- Author:      John Cleaver
-- Create date: 2019-5-31
-- Description:	Update column lengths on Donations and Recurring Gifts table to
-- avoid truncated data errors for Pushpay API update
-- =============================================

BEGIN
	ALTER TABLE Donations ALTER COLUMN Transaction_Code NVARCHAR(75) NULL;
	ALTER TABLE Donations ALTER COLUMN Subscription_Code NVARCHAR(75) NULL;
END
GO

BEGIN
	ALTER TABLE Recurring_Gifts ALTER COLUMN Subscription_ID NVARCHAR(75) NULL;
END
GO