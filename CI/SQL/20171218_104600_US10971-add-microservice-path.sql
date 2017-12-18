USE MinistryPlatform
GO

DECLARE @AppCode NVARCHAR(32) = 'CRDS-FINANCE'

IF NOT EXISTS(SELECT * FROM dp_Configuration_Settings WHERE Application_Code = @AppCode AND Key_Name = 'FinanceMicroservicePath')
BEGIN
 INSERT INTO dp_Configuration_Settings(Application_Code,Key_Name,Value,Description,Domain_ID)
	VALUES(@AppCode,'FinanceMicroservicePath','finance/api/','Url for finance microservice',1)
END

GO
