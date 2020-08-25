USE [MinistryPlatform]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Chris Andrews (Ingage)
-- Create date: 08/25/2020
-- Description:	Insert raw json for recurring schedules
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[api_crds_Insert_PushpayRecurringSchedulesRawJson]
	@RawJson nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	
	INSERT INTO [dbo].[cr_RawPushpayRecurringSchedules]
		([RawJson])
	VALUES
		(@RawJson)
END