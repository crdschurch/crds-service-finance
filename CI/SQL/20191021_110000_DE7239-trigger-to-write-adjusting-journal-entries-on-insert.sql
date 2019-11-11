USE [MinistryPlatform]
GO
/****** Object:  Trigger [dbo].[crds_tr_Update_With_Congregation_Id]    Script Date: 10/21/2019 2:38:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Matt Brewer
-- Create date: 6/10/2015
-- Description:	Update with Congregation ID on every INSERT KD: The original trigger was for update as well
-- Updates: KD 5/23/17 
--			1. Only update congregation on Update donation id (since the donor comes from the donation and the congregation comes from the donor)
--				and only if soft_credit_donor is null
--			2. Update HC_Congregation_ID to the donor's congregation id also anytime the donation id is updated
--			2. Save the soft credit donor's congregation in congregation id 
--			   On update SC donor
--				1. If the SC Donor is null (we removed it), then the Congregation ID is set to the HC Donor Congregation field 
--				2. If the SC Donor is not null (we added or changed it), then Congregation ID is set to SC donor congregation id
--			3. Add audit logs for congregation_id updates
--			KD 6/28 Remove only update on update donation id since this brakes when BMT is used (the donor isn't set on the donation yet when the dd is created)
-- =============================================

IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'crds_tr_Update_With_Congregation_Id'))
  DROP TRIGGER crds_tr_Update_With_Congregation_Id
GO 

CREATE TRIGGER [dbo].[crds_tr_Update_With_Congregation_Id]
ON [dbo].[Donation_Distributions]
AFTER INSERT, UPDATE

AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @Donation_Distribution_ID INT;
	DECLARE @New_Congregation_ID INT;
	DECLARE @Old_Congregation_ID INT;

	DECLARE @modified TABLE (
		Donation_Distribution_ID INT NOT NULL,
		New_Congregation_ID INT,
		Old_Congregation_ID INT
	);

	IF UPDATE(Donation_ID) --IF UPDATE returns the TRUE value in INSERT actions because the columns have either explicit values or implicit (NULL) values inserted.
	BEGIN
		UPDATE dd
		SET
			HC_Donor_Congregation_ID = COALESCE(h.Congregation_Id, 5),
			Congregation_ID = CASE WHEN dd.Soft_Credit_Donor IS NULL THEN COALESCE(h.Congregation_Id, 5) ELSE dd.Congregation_ID END
		OUTPUT
			inserted.Donation_Distribution_ID, inserted.Congregation_ID, deleted.Congregation_ID INTO @modified
		FROM
			[dbo].[Donation_Distributions] dd
			INNER JOIN [dbo].[Donations] d ON d.donation_id = dd.donation_id
			INNER JOIN [dbo].[Donors] do ON do.donor_id = d.donor_id
			INNER JOIN [dbo].[Contacts] c ON c.contact_id = do.contact_id
			LEFT JOIN [dbo].[Households] h ON h.household_id = c.household_id
			INNER JOIN INSERTED ON INSERTED.Donation_Distribution_ID = dd.Donation_Distribution_ID
		;
	END --end update donation id 
	

    --If the soft_credit_donor was updated, removed, or added
	IF UPDATE(Soft_Credit_Donor)  --IF UPDATE returns the TRUE value in INSERT actions because the columns have either explicit values or implicit (NULL) values inserted.
	BEGIN
		UPDATE dd
		SET
			Congregation_ID =
				CASE
					-- soft credit donor is added or modified
					WHEN dd.soft_credit_donor IS NOT NULL THEN COALESCE(h.Congregation_Id, 5)
					-- soft credit donor is deleted (existed before)
					WHEN (SELECT Soft_Credit_Donor FROM DELETED WHERE DELETED.Donation_Distribution_ID = dd.Donation_Distribution_ID)
					     IS NOT NULL THEN dd.HC_Donor_Congregation_ID
					-- otherwise, no change
					ELSE dd.Congregation_ID
				END,
			HC_Donor_Congregation_ID =
				CASE
					WHEN dd.soft_credit_donor IS NOT NULL AND dd.HC_Donor_Congregation_ID IS NULL THEN dd.Congregation_Id
					ELSE dd.HC_Donor_Congregation_ID
				END
		OUTPUT
			inserted.Donation_Distribution_ID, inserted.Congregation_ID, deleted.Congregation_ID INTO @modified
		FROM
			[dbo].[Donation_Distributions] dd
			INNER JOIN [dbo].[Donations] d ON d.donation_id = dd.donation_id
			LEFT JOIN [dbo].[Donors] do ON do.Donor_ID = dd.Soft_Credit_Donor
			LEFT JOIN [dbo].[Contacts] c ON c.contact_id = do.contact_id
			LEFT JOIN [dbo].[Households] h ON h.household_id = c.household_id
			INNER JOIN INSERTED ON INSERTED.Donation_Distribution_ID = dd.Donation_Distribution_ID
		;
	END --end if soft credit 

	DECLARE @Audit_Cursor CURSOR;
	SET @Audit_Cursor = CURSOR FOR
	SELECT
		Donation_Distribution_ID,
		New_Congregation_ID,
		Old_Congregation_ID
	FROM @modified
	WHERE New_Congregation_ID <> Old_Congregation_ID;

    OPEN @Audit_Cursor
    FETCH NEXT FROM @Audit_Cursor INTO @Donation_Distribution_ID, @New_Congregation_ID, @Old_Congregation_ID
    WHILE @@FETCH_STATUS = 0
    BEGIN
		EXEC crds_Add_Audit 
				@TableName = 'Donation_Distributions'
			,@Record_ID = @Donation_Distribution_ID
			,@Audit_Description = 'Updated'
			,@UserName = 'Svc Mngr'
			,@UserID = 0
			,@FieldName = 'Congregation_ID'
			,@FieldLabel = 'Congregation'
			,@PreviousValue = @Old_Congregation_ID
			,@NewValue = @New_Congregation_ID	

        FETCH NEXT FROM @Audit_Cursor INTO @Donation_Distribution_ID, @New_Congregation_ID, @Old_Congregation_ID
	END

	CLOSE @Audit_Cursor
	DEALLOCATE @Audit_Cursor

	--DETERMINE WHETHER THIS WAS TRIGGERED BY AN INSERT OR UPDATE
	DECLARE @Action as char(1);
    SET @Action = (CASE WHEN EXISTS(SELECT * FROM INSERTED)
                         AND EXISTS(SELECT * FROM DELETED)
                        THEN 'U'  -- Set Action to Updated.
                        WHEN EXISTS(SELECT * FROM INSERTED)
                        THEN 'I'  -- Set Action to Insert.
                        WHEN EXISTS(SELECT * FROM DELETED)
                        THEN 'D'  -- Set Action to Deleted.
                        ELSE NULL -- Skip. It may have been a "failed delete".   
                    END)
	--END DETERMINE WHETHER THIS WAS TRIGGERED BY AN INSERT OR UPDATE

	--WRITE ADJUSTING JOURNAL ENTRY IF NEW DISTRIBUTION ON EXPORTED DONATION

	DECLARE @CountOfNewRecords INT = (SELECT COUNT(*) FROM INSERTED);

	--ONLY WRITE ADJUSTING JOURNAL ENTRY UPDATE IF IT'S AN INSERT OF ONE RECORD
	IF(@Action = 'I' AND @CountOfNewRecords = 1)
	BEGIN

	  DECLARE @DonationDistributionId INT = (SELECT Donation_Distribution_ID FROM INSERTED);
	  DECLARE @DonationId INT = (SELECT [Donation_ID] FROM [Donation_Distributions] WHERE [Donation_Distribution_ID] = @DonationDistributionId);
	  DECLARE @IsExported BIT = (SELECT dbo.[crds_IsDonationExported](@DonationId));

	  IF @IsExported = 1
	  BEGIN
		DECLARE @NewAmount MONEY = (SELECT Amount FROM INSERTED);
		DECLARE @NewProgramId INT = (SELECT Program_ID FROM INSERTED);
		DECLARE @NewCongregationId INT = (SELECT TOP(1) Congregation_ID FROM Donation_Distributions dd WHERE dd.Donation_ID = @DonationId ORDER BY Donation_Distribution_ID ASC)

		DECLARE @NewGlAccountNumber VARCHAR(50) = (SELECT dbo.crds_GetGLAccountNumber(@NewProgramId, @NewCongregationId));
		DECLARE @DonationDate DATETIME = (SELECT [Donation_Date] FROM [Donations] WHERE [Donation_ID] = @DonationId);

		EXEC api_crds_CreateAdjustingEntryForAmountChanged
			  @Amount = @NewAmount,
			  @GLAccountNumber = @NewGlAccountNumber,
			  @DonationDistributionId = @DonationDistributionId,
			  @DonationDate = @DonationDate

	  END
	END --END WRITE ADJUSTING JOURNAL ENTRY
	 
END --end update trigger

