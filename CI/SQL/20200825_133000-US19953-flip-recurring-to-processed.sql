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

CREATE OR ALTER PROCEDURE [dbo].[api_crds_Set_Recurring_JSON_To_Processed]
	@RecurringGiftScheduleId nvarchar(max)
AS
BEGIN
	UPDATE cr_RawPushpayRecurringSchedules
	SET IsProcessed = 1
	WHERE RecurringGiftScheduleId = @RecurringGiftScheduleId
END