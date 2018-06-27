--select *
--from dp_Pages
--where Page_ID = 517

--select *
--from dp_Page_Views
--where Page_View_ID = 94078

--backup of original fields
--Donor_ID_Table_Contact_ID_Table.[Display_Name]    ,Donor_ID_Table_Contact_ID_Table_User_Account_Table.[User_Email]    ,Frequency_ID_Table.[Frequency]    ,CASE(Frequency_ID_Table.Frequency_ID)      WHEN 1     THEN       CONCAT(      'Every ',      Day_Of_Week_ID_Table.Day_Of_Week       )      ELSE     CONCAT(       CAST(Day_Of_Month AS VARCHAR),       CASE(Day_Of_Month % 10)      WHEN 1 THEN 'st'      WHEN 2 THEN 'nd'      WHEN 3 THEN 'rd'      ELSE 'th'       END,       ' of the month'     )      END AS Recurrence    ,Recurring_Gifts.[Start_Date]    ,Recurring_Gifts.[End_Date]    ,Recurring_Gifts.[Amount]    ,Program_ID_Table.[Program_Name]    ,Congregation_ID_Table.[Congregation_Name]    ,CONCAT(Donor_Account_ID_Table_Account_Type_ID_Table.[Account_Type], '/', Donor_Account_ID_Table.[Account_Number]    ,'/', Donor_Account_ID_Table.[Institution_Name]) AS [Donor_Account]    ,Recurring_Gifts.[Subscription_ID]    ,Recurring_Gifts.Source_Url    ,Recurring_Gifts.Predefined_Amount

--updated fields
--Donor_ID_Table_Contact_ID_Table.[Display_Name]    ,Donor_ID_Table_Contact_ID_Table_User_Account_Table.[User_Email]    ,Frequency_ID_Table.[Frequency]    , CASE Frequency_ID_Table.[Frequency_ID] WHEN 1 THEN CONCAT(REPLACE(Day_Of_Week_ID_Table.Day_Of_Week, ' ', '') , 's Weekly')   WHEN 2 THEN CONCAT('Monthly (', Recurring_Gifts.[Day_Of_Month],   CASE  WHEN (Recurring_Gifts.[Day_Of_Month] % 100) IN (11, 12, 13) THEN 'th'   ELSE CASE (Recurring_Gifts.[Day_Of_Month] % 10)  WHEN 1 THEN 'st' WHEN 2 THEN 'nd' WHEN 3 THEN 'rd' ELSE 'th'		    END	END  ,')' ) WHEN 3 THEN '1st and 15th' WHEN 4 THEN 'Every Other Week' ELSE Frequency_ID_Table.[Frequency] END AS [Recurrence]    ,Recurring_Gifts.[Start_Date]    ,Recurring_Gifts.[End_Date]    ,Recurring_Gifts.[Amount]    ,Program_ID_Table.[Program_Name]    ,Congregation_ID_Table.[Congregation_Name]    ,CONCAT(Donor_Account_ID_Table_Account_Type_ID_Table.[Account_Type], '/', Donor_Account_ID_Table.[Account_Number]    ,'/', Donor_Account_ID_Table.[Institution_Name]) AS [Donor_Account]    ,Recurring_Gifts.[Subscription_ID]    ,Recurring_Gifts.Source_Url    ,Recurring_Gifts.Predefined_Amount


USE [MinistryPlatform]
GO
UPDATE [dbo].[dp_Pages]
SET [Default_Field_List] = 'Donor_ID_Table_Contact_ID_Table.[Display_Name]    ,Donor_ID_Table_Contact_ID_Table_User_Account_Table.[User_Email]    ,Frequency_ID_Table.[Frequency]    , CASE Frequency_ID_Table.[Frequency_ID] WHEN 1 THEN CONCAT(REPLACE(Day_Of_Week_ID_Table.Day_Of_Week, '' '', '''') , ''s Weekly'')   WHEN 2 THEN CONCAT(''Monthly ('', Recurring_Gifts.[Day_Of_Month],   CASE  WHEN (Recurring_Gifts.[Day_Of_Month] % 100) IN (11, 12, 13) THEN ''th''   ELSE CASE (Recurring_Gifts.[Day_Of_Month] % 10)  WHEN 1 THEN ''st'' WHEN 2 THEN ''nd'' WHEN 3 THEN ''rd'' ELSE ''th''		    END	END  ,'')'' ) WHEN 3 THEN ''1st and 15th'' WHEN 4 THEN ''Every Other Week'' ELSE Frequency_ID_Table.[Frequency] END AS [Recurrence]    ,Recurring_Gifts.[Start_Date]    ,Recurring_Gifts.[End_Date]    ,Recurring_Gifts.[Amount]    ,Program_ID_Table.[Program_Name]    ,Congregation_ID_Table.[Congregation_Name]    ,CONCAT(Donor_Account_ID_Table_Account_Type_ID_Table.[Account_Type], ''/'', Donor_Account_ID_Table.[Account_Number]    ,''/'', Donor_Account_ID_Table.[Institution_Name]) AS [Donor_Account]    ,Recurring_Gifts.[Subscription_ID]    ,Recurring_Gifts.Source_Url    ,Recurring_Gifts.Predefined_Amount'
WHERE [Page_ID] = 517;
GO