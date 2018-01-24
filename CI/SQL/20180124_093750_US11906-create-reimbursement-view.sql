USE [MinistryPlatform]
GO

DECLARE @PageViewID int = 2311

IF NOT EXISTS(SELECT * FROM dp_Page_Views WHERE Page_View_ID = @PageViewID)
BEGIN
    SET IDENTITY_INSERT dp_Page_Views ON
    INSERT INTO [dbo].[dp_Page_Views]
           ([Page_View_ID]
           ,[View_Title]
           ,[Page_ID]
           ,[Description]
           ,[Field_List]
           ,[View_Clause]
           ,[Order_By])
     VALUES
           (@PageViewID
           ,'Reimbursed Donations'
           ,297
           ,null
           ,'Donations.[Donation_ID] AS [Donation ID]
, Donations.[Donation_Date] AS [Donation Date]
, Donations.[Donation_Amount] AS [Donation Amount]
, Payment_Type_ID_Table.[Payment_Type] AS [Payment Type]
, Donations.[Transaction_Code] AS [Transaction Code]
, Donor_ID_Table_Contact_ID_Table.[Display_Name] AS [Display Name]
, Donor_ID_Table_Contact_ID_Table.[Nickname] AS [Nickname]
, Donor_ID_Table_Contact_ID_Table.[First_Name] AS [First Name]
, Batch_ID_Table.[Batch_Name] AS [Batch Name]
, Donation_Status_ID_Table.[Donation_Status] AS [Donation Status]
, Batch_ID_Table_Batch_Entry_Type_ID_Table.[Batch_Entry_Type] AS [Batch Entry Type]'
           ,'Payment_Type_ID_Table.[Payment_Type] = ''Reimbursement'''
           ,'Donations.[Donation_Date]  DESC')
    SET IDENTITY_INSERT dp_Page_Views OFF
END

