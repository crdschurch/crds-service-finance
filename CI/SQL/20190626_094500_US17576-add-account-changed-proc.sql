-- =============================================
-- Author:      John Cleaver
-- Create date: 2019-06-26
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
	@Amount MONEY
AS
-- old account adjustment
	INSERT INTO cr_Adjusting_Journal_Entries
		([Created_Date],
		[Sent_To_GL_date],
		[GL_Account_Number],
		[Amount],
		[Adjustment],
		[Description],
		[Donation_Distribution_ID],
		[Domain_ID])
	VALUES
		(GETDATE(),
		NULL,
		@OldAccountNumber,
		-@Amount,
		(SELECT convert(varchar(7), getdate(), 126)) + ' revenue reclassifications', -- show YYYY-MM format
		'Account Adjustment (debit) for ' + @OldAccountNumber,
		@DonationDistributionId,
		1)

-- new account adjustment
	INSERT INTO cr_Adjusting_Journal_Entries
		([Created_Date],
		[Sent_To_GL_date],
		[GL_Account_Number],
		[Amount],
		[Adjustment],
		[Description],
		[Donation_Distribution_ID],
		[Domain_ID])
	VALUES
		(GETDATE(),
		NULL,
		@NewAccountNumber,
		+@Amount,
		(SELECT convert(varchar(7), getdate(), 126)) + ' revenue reclassifications', -- show YYYY-MM format
		'Account Adjustment (credit) for ' + @NewAccountNumber,
		@DonationDistributionId,
		1)
GO