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
	@DonationDistributionId INT
AS
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
		@GLAccountNumber,
		@Amount,
		'Adjustment Placeholder',
		'Description Placeholder',
		@DonationDistributionId,
		1)
GO