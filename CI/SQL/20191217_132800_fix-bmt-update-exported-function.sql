USE [MinistryPlatform]
GO

/****** Object:  UserDefinedFunction [dbo].[crds_IsDonationExported]    Script Date: 12/17/2019 9:46:27 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER FUNCTION [dbo].[crds_IsDonationExported](@Donation_ID INT)
RETURNS NVARCHAR(20)
AS
BEGIN
	IF EXISTS ((SELECT TOP(1) Exported
		 FROM Donations d
		 INNER JOIN Batches b ON d.Batch_ID = b.Batch_ID 
		 INNER JOIN Deposits dep ON b.Deposit_ID = dep.Deposit_ID
		 WHERE Donation_ID = @Donation_ID))
		 RETURN 1
	RETURN 0
END

GO


