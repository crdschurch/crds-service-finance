USE MinistryPlatform
GO

-- =============================================
-- Author:      John Cleaver
-- Create date: 2018-11-01
-- Description:	Create non-processed batch for Declined or Offset Donations
-- =============================================


CREATE OR ALTER PROCEDURE [dbo].[crds_service_batch_declined_donations]
AS
BEGIN

DECLARE @Donation_Audit_Records dbo.crds_Audit_Item;

-- get donations data for declined or offset donations
DECLARE @tableDonations TABLE
(
	Donation_ID INT,
	Donation_Amount MONEY
)

-- select unbatched Pushpay donations that are Declined or Offset
INSERT INTO @tableDonations
SELECT Donation_ID, Donation_Amount FROM Donations
WHERE Donation_Status_ID IN (3, 6) -- Declined or Offset
AND Transaction_Code NOT LIKE 'py_%'
AND Transaction_Code NOT LIKE 'pyr_%'
AND Batch_ID IS NULL

-- exit out of this if there's no donations
IF ((SELECT COUNT(*) FROM @tableDonations) = 0)
RETURN

DECLARE @donationsSum MONEY
SELECT @donationsSum = SUM(Donation_Amount)FROM @tableDonations

DECLARE @DepositName NVARCHAR(30) = 'PPD' 
			+ FORMAT(GETDATE(), 'yyyy')
			+ FORMAT(GETDATE(), 'MM')
			+  FORMAT(GETDATE(), 'dd')
			+ FORMAT(GETDATE(), 'hh')
			+ FORMAT(GETDATE(), 'mm')

BEGIN TRANSACTION

-- add the deposit to the audit log
DECLARE @Deposits_Activity_Log dbo.crds_Audit_Item

INSERT INTO Deposits 
	(
		Deposit_Name,
		Deposit_Total,
		Deposit_Amount,
		Processor_Fee_Total,
		Deposit_Date,
		Account_Number,
		Batch_Count,
		Domain_ID,
		Exported,
		Notes,
		Staged
	)
OUTPUT 
	'Deposits', 
	INSERTED.Deposit_ID,
	'Created',
	null,
	null,
	null,
	null,
	null,
	null
INTO @Deposits_Activity_Log
VALUES
	(
		@DepositName,
		@donationsSum,
		@donationsSum,
		0,
		CONVERT(VARCHAR, GETDATE(), 20),
		' ', -- account number is set to an empty string in MP
		1,
		1,
		1,
		'Batching Pushpay Declines',
		1
	)

DECLARE @Deposit_ID INT
SELECT @Deposit_ID = Record_ID FROM @Deposits_Activity_Log

DECLARE @date DATETIME = GETDATE();
EXEC crds_Add_Audit_Items @Deposits_Activity_Log, @date, 'Svc Mngr', 0;

-- add the batch to the audit log
DECLARE @Batches_Activity_Log dbo.crds_Audit_Item

INSERT INTO Batches
	(
		Batch_Name,
		Setup_Date,
		Batch_Total,
		Item_Count,
		Batch_Entry_Type_ID,
		Deposit_ID,
		Finalize_Date,
		Domain_ID
	)
OUTPUT 
	'Batches', 
	INSERTED.Batch_ID,
	'Created',
	null,
	null,
	null,
	null,
	null,
	null
INTO @Batches_Activity_Log
VALUES
	(
		@DepositName,
		CONVERT(VARCHAR, GETDATE(), 20),
		@donationsSum,
		(SELECT COUNT(*) FROM @tableDonations),
		2, -- 2 is "Manual Entry" type
		@Deposit_ID,
		CONVERT(VARCHAR, GETDATE(), 20),
		1
	)

DECLARE @Batch_ID INT
SELECT @Batch_ID = Record_ID FROM @Batches_Activity_Log

EXEC crds_Add_Audit_Items @Batches_Activity_Log, @date, 'Svc Mngr', 0;

-- update donations with the correct batch ID
UPDATE Donations SET Batch_ID = @Batch_ID
WHERE Donation_ID IN (SELECT Donation_ID FROM @tableDonations)

-- add the batch to the audit log
DECLARE @Donations_Activity_Log dbo.crds_Audit_Item

INSERT INTO @Donations_Activity_Log
SELECT 
	'Donations', 
	Donation_ID, 
	'Updated',
	'Batch_ID',
	'Batch',
	null,
	@Batch_ID,
	null,
	null 
FROM @tableDonations

EXEC crds_Add_Audit_Items @Donations_Activity_Log, @date, 'Svc Mngr', 0;

COMMIT TRANSACTION

END