USE [MinistryPlatform]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		John Cleaver (Ingage)
-- Create date: 08/12/2020
-- Description:	Insert raw json for Donations
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[api_crds_Insert_PushpayDonationsRawJson]
	@RawJson nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
​
	INSERT INTO [dbo].[cr_RawPushpayDonations]
		([RawJson])
	VALUES
		(@RawJson)
END