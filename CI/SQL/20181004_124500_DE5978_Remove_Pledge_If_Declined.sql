USE [MinistryPlatform]
GO

DROP TRIGGER IF EXISTS [dbo].[crds_tr_Update_RemovePledgeIfDeclined]
GO

CREATE TRIGGER [dbo].[crds_tr_Update_RemovePledgeIfDeclined]
  ON  [dbo].[Donations]
  AFTER UPDATE
AS
BEGIN
	-- =============================================
	-- Author:      Joe Kerstanoff
	-- Create date: 3/10/2016
	-- Description: Remove Pledges from declined donations so they are not counted on reports / profile give page.
	-- Update: 10/4/2018 Brian Cavanaugh
	-- Update Description: Removed existing CURSOR to prevent deadlocks on key tables.  Used an UPDATE statement instead.
	-- =============================================

	UPDATE Donation_Distributions
	SET Pledge_ID = NULL
	WHERE Donation_ID in 
				(SELECT i.Donation_ID 
					FROM inserted i 
					JOIN deleted d on d.Donation_ID = i.Donation_ID 
					WHERE i.Donation_Status_ID = 3
				)

END
GO

ALTER TABLE [dbo].[Donations] ENABLE TRIGGER [crds_tr_Update_RemovePledgeIfDeclined]
GO

