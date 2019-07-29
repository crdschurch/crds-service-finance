-- =============================================
-- Author:      John Cleaver
-- Create date: 2019-06-25
-- Description:	Create Adjustments for Amount Changed when
-- Donation Distribution amounts change
-- =============================================

USE [MinistryPlatform]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].api_crds_CreateAdjustingEntryForAmountChanged
	@Amount MONEY,
	@GLAccountNumber NVARCHAR(20),
	@DonationDistributionId INT,
	@DonationDate DATETIME
AS
	INSERT INTO cr_Adjusting_Journal_Entries
		([Created_Date],
		[Donation_Date],
		[Sent_To_GL_date],
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
		@GLAccountNumber,
		@Amount,
		(SELECT convert(varchar(7), getdate(), 126)) + ' revenue reclassifications', -- show YYYY-MM format
		'Amount Adjustment for ' + @GLAccountNumber,
		@DonationDistributionId,
		1)
GO