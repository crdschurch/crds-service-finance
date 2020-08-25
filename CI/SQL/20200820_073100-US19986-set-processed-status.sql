USE [MinistryPlatform]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:      John Cleaver
-- Create date: 08/20/2020
-- Description: Flip the is processed to true 
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[api_crds_Set_Donation_JSON_To_Processed]
	@DonationId int
AS
BEGIN
	UPDATE cr_RawPushpayDonations
	SET IsProcessed = 1
	WHERE DonationId = @DonationId
END
GO


