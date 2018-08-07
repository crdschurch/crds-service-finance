--ACH Recurring Schedules Export
USE MinistryPlatform

SELECT da.Processor_ID
	, rg.Subscription_ID
	, p.Program_Name
FROM Recurring_Gifts rg
	JOIN Donor_Accounts da on rg.Donor_Account_ID = da.Donor_Account_ID
	JOIN Programs p on rg.Program_ID = p.Program_ID
WHERE rg.Start_Date >= '2017-01-01' 
	AND rg.End_Date IS NULL 
	AND da.Processor_ID LIKE 'cus_%' 
	AND rg.Subscription_ID LIKE 'sub_%'
	AND da.Account_Type_ID <> 3
