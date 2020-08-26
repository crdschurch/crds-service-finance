-- ===============================================================
-- Authors: John Cleaver <john.cleaver@contractor.crossroads.net>
-- Create date: 2020-08-13
-- Description:	Add API client permissions
-- ===============================================================

IF NOT EXISTS(SELECT * FROM [dbo].[dp_API_Procedures] WHERE [procedure_name] = 'api_crds_Insert_PushpayDonationsRawJson')
BEGIN
	DECLARE @IDs TABLE (ID INT)

	INSERT INTO [dbo].[dp_API_Procedures] ([procedure_name]) 
	OUTPUT INSERTED.API_Procedure_ID INTO @IDs
	VALUES('api_crds_Insert_PushpayDonationsRawJson')

	IF NOT EXISTS(select * from [dbo].[dp_Role_API_Procedures] WHERE API_Procedure_ID = (SELECT TOP(1) * FROM @IDs) AND Role_ID = 1023)
	BEGIN
		INSERT INTO [dbo].[dp_Role_API_Procedures]
		([Role_ID],
		[API_Procedure_ID],
		[Domain_ID])
		VALUES
		(1023,
		(SELECT TOP(1) * FROM @IDs),
		1)
	END
END