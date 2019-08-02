USE MinistryPlatform
GO

IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'crds_tr_Write_Distribution_Adjustments'))
  DROP TRIGGER crds_tr_Write_Distribution_Adjustments
GO 

CREATE TRIGGER crds_tr_Write_Distribution_Adjustments
ON Donation_Distributions
AFTER UPDATE
AS
 
IF (UPDATE(Amount) OR UPDATE(Program_ID) OR UPDATE(Congregation_ID))
  BEGIN
	DECLARE @DonationDistributionId INT = (SELECT Donation_Distribution_ID FROM INSERTED);
	DECLARE @DonationId INT = (SELECT [Donation_ID] FROM [Donation_Distributions] WHERE [Donation_Distribution_ID] = @DonationDistributionId);
	DECLARE @IsExported BIT = (SELECT dbo.[crds_IsDonationExported](@DonationId));

	IF @IsExported = 1
	  BEGIN
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

		DECLARE @DonationDate DATETIME = (SELECT [Donation_Date] FROM [Donations] WHERE [Donation_ID] = @DonationId);

		IF(@DidAmountChange = 1)
		  BEGIN
			EXEC api_crds_CreateAdjustingEntryForAmountChanged
			  @Amount = @AmountChange,
			  @GLAccountNumber = @OldGlAccountNumber,
			  @DonationDistributionId = @DonationDistributionId,
			  @DonationDate = @DonationDate
			END
    
		IF(@DidAccountNumberChange = 1)
		  BEGIN
			EXEC [dbo].api_crds_CreateAdjustingEntryForAccountChanged
			@OldAccountNumber = @OldGlAccountNumber,
			@NewAccountNumber = @NewGlAccountNumber,
			@DonationDistributionId = @DonationDistributionId,
			@Amount = @NewAmount,
			@DonationDate = @DonationDate
		  END
	END
  END
