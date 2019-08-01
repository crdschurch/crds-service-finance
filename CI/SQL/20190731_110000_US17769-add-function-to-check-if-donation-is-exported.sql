USE [MinistryPlatform]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER FUNCTION [dbo].[crds_IsDonationExported](@Donation_ID INT)
RETURNS NVARCHAR(20)
AS
BEGIN
	RETURN 
		(SELECT TOP(1) Exported
		 FROM Donations d
		 INNER JOIN Batches b ON d.Batch_ID = b.Batch_ID 
		 INNER JOIN Deposits dep ON b.Deposit_ID = dep.Deposit_ID
		 WHERE Donation_ID = @Donation_ID)
END

GO
