-- ACH Recurring Schedules Export
USE MinistryPlatform

SELECT da.Processor_ID as 'Stripe Customer ID'
	, rg.Subscription_ID as 'Stripe Subscription ID'
	, 'Crossroads' as 'Merchant Name'
	, p.Program_Name as 'Fund Name (optional)'
	, CASE rgf.Frequency_ID 
		WHEN 1 THEN CONCAT('Every Week ',  RTRIM(rd.Day_Of_Week)) 
		ELSE CONCAT('Every Month ', CONVERT(varchar, rg.Day_Of_Month)) 
	  END AS "Frequency"
	, rg.Amount as 'Amount'
	, da.Routing_Number as 'Routing Number'
	, da.Account_Number as 'Account Number'
	, [at].Account_Type as 'Account Type'
	, ISNULL(c.Nickname, c.First_Name) as 'First Name'
	, c.Last_Name as 'Last Name'
	, c.Email_Address as 'Email (optional)'
	, a.Address_Line_1 as 'Street (optional)'
	, a.City as 'City (optional)'
	, a.[State/Region] as 'State (optional)'
	, a.Postal_Code as 'Zip (optional)'
	, c.Mobile_Phone as 'Phone (optional)'
	, rg.Recurring_Gift_ID as 'Recurring Gift ID' -- not used by Pushpay, for CR use
	, da.Donor_ID as 'Donor ID'
FROM Recurring_Gifts rg
	JOIN Contacts c on rg.Donor_ID = c.Donor_Record
	JOIN Programs p on rg.Program_ID = p.Program_ID
	JOIN Recurring_Gift_Frequencies rgf on rg.Frequency_ID = rgf.Frequency_ID
	JOIN Donor_Accounts da on rg.Donor_Account_ID = da.Donor_Account_ID
	JOIN Account_Types [at] on da.Account_Type_ID = [at].Account_Type_ID
	LEFT OUTER JOIN Households h on c.Household_ID = h.Household_ID
	LEFT OUTER JOIN Addresses a on h.Address_ID = a.Address_ID	
	LEFT OUTER JOIN Recurring_Gift_Days rd ON rd.Day_Of_Week_ID = rg.Day_Of_Week_ID
WHERE rg.End_Date IS NULL 
	AND da.Processor_ID LIKE 'cus_%' 
	AND rg.Subscription_ID LIKE 'sub_%'
	AND da.Account_Type_ID <> 3