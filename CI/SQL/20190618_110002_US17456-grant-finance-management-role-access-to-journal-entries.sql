USE [MinistryPlatform]
GO

DECLARE @Page_ID INT = 650;
DECLARE @Role_Id INT;
SELECT @Role_Id = Role_ID FROM dp_Roles WHERE Role_Name = 'Finance Management - CRDS';

IF NOT EXISTS(SELECT * FROM dp_role_pages WHERE Page_ID = @Page_ID and Role_ID = @Role_Id )
BEGIN
	INSERT INTO dp_role_pages(Role_ID, Page_ID, Access_Level, Scope_All, Approver, File_Attacher, Data_Importer, Data_Exporter, Secure_Records, Allow_Comments, Quick_Add)
	VALUES(@Role_Id, @Page_ID, 3, 0,0,0,0,1,0,0,1)
END