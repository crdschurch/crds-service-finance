USE [MinistryPlatform]
GO

-- =============================================
-- Author:      John Cleaver
-- Create date: 2018-08-24
-- Description:	Add Stripe and Pushpay Recurring Givers Page View
-- =============================================

DECLARE @PageViewID int = 1140

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
		   ,'Stripe and Pushpay Recurring Givers'
           ,299
           ,'Page View Showing Givers with Both Active Stripe and Pushpay Recurring Gifts'
           ,'(SELECT DISTINCT [Donor_ID]) AS [Donor ID] , Contact_ID_Table.[Display_Name] AS [Display Name] , (SELECT COUNT(Recurring_Gift_ID) FROM Recurring_Gifts r WHERE r.Donor_ID = Donors.[Donor_ID] AND Subscription_ID LIKE ''sub_%'' AND End_Date IS NULL) AS [Stripe Gifts] , (SELECT COUNT(Recurring_Gift_ID) FROM Recurring_Gifts r WHERE r.Donor_ID = Donors.[Donor_ID] AND Subscription_ID NOT LIKE ''sub_%'' AND End_Date IS NULL) AS [Pushpay Gifts] , (CASE   WHEN        (SELECT COUNT(Recurring_Gift_ID) FROM Recurring_Gifts r WHERE r.Donor_ID = Donors.[Donor_ID] AND Subscription_ID LIKE ''sub_%'' AND End_Date IS NULL) > 0     AND     (SELECT COUNT(Recurring_Gift_ID) FROM Recurring_Gifts r WHERE r.Donor_ID = Donors.[Donor_ID] AND Subscription_ID NOT LIKE ''sub_%'' AND End_Date IS NULL) > 0      THEN ''Y''   ELSE ''N''  END  ) AS [Has Both Gift Types] '
           ,'1=1'
           ,NULL)
	SET IDENTITY_INSERT dp_Page_Views OFF
END
