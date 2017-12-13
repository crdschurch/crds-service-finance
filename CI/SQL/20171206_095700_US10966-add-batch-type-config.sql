USE MinistryPlatform
GO

DECLARE @AppCode NVARCHAR(32) = 'CRDS-FINANCE'

IF NOT EXISTS(SELECT * FROM dp_Configuration_Settings WHERE Application_Code = @AppCode AND Key_Name = 'BatchEntryType')
BEGIN
 INSERT INTO dp_Configuration_Settings(Application_Code,Key_Name,Value,Description,Domain_ID)
	VALUES(@AppCode,'BatchEntryType','10','Type for payment processor batch entry',1)
END

GO
