USE MinistryPlatform
GO

IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'crds_tr_Write_Adjusting_Journal_Entries'))
  DROP TRIGGER crds_tr_Write_Adjusting_Journal_Entries
GO 

CREATE TRIGGER crds_tr_Write_Adjusting_Journal_Entries
ON Donation_Distributions
AFTER UPDATE
AS

-- get the donation and check to see if it's exported already - this should run only on Donations where
-- the Deposit has already been exported
DECLARE @ExportedStatus BIT = 0

SELECT TOP(1) @ExportedStatus = dep.Exported FROM Donations d INNER JOIN Batches b
ON d.Batch_ID = b.Batch_ID INNER JOIN Deposits dep ON b.Deposit_ID = dep.Deposit_ID
WHERE d.Donation_ID = Donation_ID
 
IF (UPDATE(Amount) OR UPDATE(Program_ID) OR UPDATE(Congregation_ID)) AND @ExportedStatus = 1
BEGIN

	DECLARE @DonationDistributionId INT = (SELECT Donation_Distribution_ID FROM INSERTED);

	DECLARE @OldAmount MONEY = (SELECT Amount FROM DELETED);
    DECLARE @NewAmount MONEY = (SELECT Amount FROM INSERTED);
	DECLARE @AmountChange MONEY = @NewAmount - @OldAmount;

	DECLARE @OldProgramId INT = (SELECT Program_ID FROM DELETED);
	DECLARE @NewProgramId INT = (SELECT Program_ID FROM INSERTED);
	
	DECLARE @OldCongregationId INT = (SELECT Congregation_ID FROM DELETED);
	DECLARE @NewCongregationId INT = (SELECT Congregation_ID FROM INSERTED);

	DECLARE @OldGlAccountNumber VARCHAR(50) = (SELECT dbo.crds_GetGLAccountNumber(@OldProgramId, @OldCongregationId));
	DECLARE @NewGlAccountNumber VARCHAR(50) = (SELECT dbo.crds_GetGLAccountNumber(@NewProgramId, @NewCongregationId)); 

	DECLARE @DidAmountChange BIT = (CASE WHEN @NewAmount = @OldAmount THEN 0 ELSE 1 END);
	DECLARE @DidAccountNumberChange BIT = (CASE WHEN @NewGlAccountNumber = @OldGlAccountNumber THEN 0 ELSE 1 END);

	IF(@DidAmountChange = 1)
		BEGIN
			EXEC api_crds_CreateAdjustingEntryForAmountChanged
				@Amount = @AmountChange,
				@GLAccountNumber = @OldGlAccountNumber,
				@DonationDistributionId = @DonationDistributionId
		END
    
	IF(@DidAccountNumberChange = 1)
	  BEGIN
		EXEC [dbo].api_crds_CreateAdjustingEntryForAccountChanged
		@OldAccountNumber = @OldGlAccountNumber,
		@NewAccountNumber = @NewGlAccountNumber,
		@DonationDistributionId = @DonationDistributionId,
		@Amount = @NewAmount
	  END
END
