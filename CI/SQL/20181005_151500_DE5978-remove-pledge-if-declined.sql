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
	-- Update 10/5/2018 Chris Branch
	-- Update Description: Added column level restrictions to reduce how often the trigger runs, there by reducing the
	--						liklihood that dead locks are caused.
	-- =============================================

	IF UPDATE(Donation_Status_ID)
    BEGIN
        DECLARE @Declined_Donations TABLE (
            Donation_ID INT NOT NULL
        );

        -- get collection of donations that are now declined, but have a pledge id
        INSERT INTO @Declined_Donations
            (Donation_ID)
        SELECT
            i.Donation_ID
        FROM
            INSERTED i
			INNER JOIN Donation_Distributions dd ON dd.Donation_ID = i.Donation_ID
        WHERE
            i.Donation_Status_ID = 3    -- declined
            AND dd.Pledge_ID IS NOT NULL

        IF EXISTS (SELECT 1 FROM @Declined_Donations)
        BEGIN
            UPDATE Donation_Distributions
            SET Pledge_ID = NULL
            WHERE Donation_ID IN (SELECT Donation_ID FROM @Declined_Donations);
        END
    END
END
GO

ALTER TABLE [dbo].[Donations] ENABLE TRIGGER [crds_tr_Update_RemovePledgeIfDeclined]
GO
