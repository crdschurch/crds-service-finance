USE MinistryPlatform
GO

-- ===============================================================
-- Authors: John Cleaver <john.cleaver@ingagepartners.com>
-- Create date: 9/6/2019
-- Description:	Add configuration setting for journal entry endpoint
-- ===============================================================

DECLARE @AppCode NVARCHAR(32) = 'CRDS-FINANCE'

IF NOT EXISTS(SELECT * FROM dp_Configuration_Settings WHERE Application_Code = @AppCode AND Key_Name = 'JournalEntryExportUrl')
BEGIN
 INSERT INTO dp_Configuration_Settings(Application_Code,Key_Name,Value,Description,Domain_ID)
	VALUES(@AppCode,'JournalEntryExportUrl','test_value','Velosio endpoint for Journal Entry export',1)
END

GO