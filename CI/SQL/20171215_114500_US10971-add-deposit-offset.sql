USE MinistryPlatform
GO

DECLARE @AppCode NVARCHAR(32) = 'CRDS-FINANCE'

IF NOT EXISTS(SELECT * FROM dp_Configuration_Settings WHERE Application_Code = @AppCode AND Key_Name = 'DepositProcessingOffset')
BEGIN
 INSERT INTO dp_Configuration_Settings(Application_Code,Key_Name,Value,Description,Domain_ID)
	VALUES(@AppCode,'DepositProcessingOffset','10','How many days prior to current date are checked for outstanding settlements',1)
END

GO
