USE MinistryPlatform
GO

DECLARE @AppCode NVARCHAR(32) = 'CRDS-FINANCE'
DECLARE @KeyName NVARCHAR(32) = 'PushpayJobDelayMinutes'
DECLARE @Description NVARCHAR(2000) = 'Number of minutes to wait to retry Pushpay jobs created by Hangfire'

IF NOT EXISTS(SELECT * FROM dp_Configuration_Settings WHERE Application_Code = @AppCode AND Key_Name = @KeyName)
BEGIN
 INSERT INTO dp_Configuration_Settings(Application_Code,Key_Name,Value,Description,Domain_ID)
	VALUES(@AppCode,@KeyName,1,@Description,1)
END

GO
