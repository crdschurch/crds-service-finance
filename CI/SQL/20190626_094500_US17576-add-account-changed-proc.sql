-- =============================================
-- Author:      John Cleaver
-- Create date: 2019-07-30
-- Description:	Create Adjustments for Program and/or Congregation 
-- changes on a Donation Distribution
-- =============================================

USE [MinistryPlatform]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].api_crds_CreateAdjustingEntryForAccountChanged
	@OldAccountNumber NVARCHAR(20),
	@NewAccountNumber NVARCHAR(20),
	@DonationDistributionId INT,
	@Amount MONEY,
	@DonationDate DATETIME
AS
-- old account adjustment
	INSERT INTO cr_Distribution_Adjustments
		([Created_Date],
		[Donation_Date],
		[Processed_Date],
		[GL_Account_Number],
		[Amount],
		[Adjustment],
		[Description],
		[Donation_Distribution_ID],
		[Domain_ID])
	VALUES
		(GETDATE(),
		@DonationDate,
		NULL,
		@OldAccountNumber,
		-@Amount,
		(SELECT convert(varchar(7), getdate(), 126)) + ' revenue reclassifications', -- show YYYY-MM format
		'Account Adjustment (debit) for ' + @OldAccountNumber,
		@DonationDistributionId,
		1)

-- new account adjustment
	INSERT INTO cr_Distribution_Adjustments
		([Created_Date],
		[Donation_Date],
		[Processed_Date],
		[GL_Account_Number],
		[Amount],
		[Adjustment],
		[Description],
		[Donation_Distribution_ID],
		[Domain_ID])
	VALUES
		(GETDATE(),
		@DonationDate,
		NULL,
		@NewAccountNumber,
		+@Amount,
		(SELECT convert(varchar(7), getdate(), 126)) + ' revenue reclassifications', -- show YYYY-MM format
		'Account Adjustment (credit) for ' + @NewAccountNumber,
		@DonationDistributionId,
		1)
GO