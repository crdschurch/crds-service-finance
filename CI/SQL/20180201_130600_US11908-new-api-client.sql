USE MinistryPlatform
GO

IF NOT EXISTS(SELECT * FROM [dbo].[dp_API_Clients] WHERE [Client_ID] = 'CRDS.Service.Finance')

BEGIN
	-- insert SECURITY ROLE
	DECLARE @tableRole table (Role_ID int);
	INSERT INTO dp_Roles (Role_Name,Description,Domain_ID)
	OUTPUT INSERTED.Role_ID INTO @tableRole
	VALUES ('Finance Microservice','Grants access for usage in the finance microservice',1);
	
	DECLARE @role_id int;
	SELECT @role_id = Role_ID from @tableRole;

	-- insert CONTACT
	DECLARE @tableContact table (Contact_ID int);
	INSERT INTO Contacts (Display_Name,Contact_Status_ID,Email_Address,Company,Domain_ID)
	OUTPUT INSERTED.Contact_ID INTO @tableContact
	VALUES ('Finance Microservice',1,'webteam+FinanceMicroservice@crossroads.net',0,1);

	DECLARE @contact_id int;
	SELECT @contact_id = Contact_ID from @tableContact;

	-- insert USER
	DECLARE @tableUser table (User_ID int);
	INSERT INTO dp_Users (User_Name,User_Email,Admin,Domain_ID,Contact_ID)
	OUTPUT INSERTED.User_ID INTO @tableUser
	VALUES ('webteam+FinanceMicroservice@crossroads.net','webteam+FinanceMicroservice@crossroads.net',0,1,@contact_id);

	DECLARE @user_id int;
	SELECT @user_id = User_ID from @tableUser;

	-- insert USER SECURITY ROLE
	INSERT INTO dp_User_Roles (User_ID,Role_ID,Domain_ID)
	VALUES (@user_id,@role_id,1);

	-- insert API Procedure access for Role
	-- get the role needed (existing)
	DECLARE @apiProcedureId int;
	SELECT @apiProcedureId = API_Procedure_ID from dp_API_Procedures where Procedure_Name='api_Common_FindMatchingContact'

	INSERT INTO dp_Role_API_Procedures (Role_ID,API_Procedure_ID,Domain_ID)
	VALUES (@role_id,@apiProcedureId,1);

	-- insert API CLIENT
	INSERT INTO dp_API_Clients (Display_Name,Client_ID,Token_Lifetime,Client_User_ID,Domain_ID)
	VALUES ('CRDS Finance Microservice','CRDS.Service.Finance',30,@user_id,1);
END
