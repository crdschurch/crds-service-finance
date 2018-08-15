------------  People File ------------------------

 /*
  * Exports person information for credit card
  * recurring gift contacts. This should be used in
  * conjunction with the credit-card-schedules-export
  * where "Person ID" columns match up
  */


 /*
  * Email address is modified so that emails do not
  * get sent out when the result file is imported
  * in PushPay's system
  */

USE MinistryPlatform

SELECT DISTINCT
	c.Contact_GUID AS "Person ID" -- TODO only needed if person file
	, c.Nickname AS "First Name"
	, c.Last_Name AS "Last Name"
	-- TODO make sure this is set correctly
	--, c.Email_Address AS "Email"
	, CONCAT(SUBSTRING(c.Email_Address, 1, CHARINDEX('@', c.Email_Address)-1), '_____', SUBSTRING(c.Email_Address, CHARINDEX('@', c.Email_Address), 1000)) AS "Email"
	, LTRIM(RTRIM(COALESCE(c.Mobile_Phone, ''))) AS "Mobile Number"
	, a.Address_Line_1 as "Address 1"
	, a.City as "City"
	, a.[State/Region] as "State"
	, a.Postal_Code as "Zip"
FROM
	[MinistryPlatform].[dbo].[Contacts] c
	LEFT OUTER JOIN [MinistryPlatform].[dbo].[Households] h ON h.Household_ID = c.Household_ID
	LEFT OUTER JOIN [MinistryPlatform].[dbo].[Addresses] a ON h.Address_ID = a.Address_ID
	JOIN [MinistryPlatform].[dbo].[Recurring_Gifts] rg ON rg.Donor_ID = c.Donor_Record
	JOIN [MinistryPlatform].[dbo].[Programs] p ON p.Program_ID = rg.Program_ID
	JOIN [MinistryPlatform].[dbo].[Recurring_Gift_Frequencies] rgf ON rgf.Frequency_ID = rg.Frequency_ID
	JOIN [MinistryPlatform].[dbo].[Donor_Accounts] da ON rg.Donor_Account_ID = da.Donor_Account_ID
	LEFT OUTER JOIN [MinistryPlatform].[dbo].[Recurring_Gift_Days] rd ON rd.Day_Of_Week_ID = rg.Day_Of_Week_ID
WHERE
	rg.Start_Date <= GETDATE()
	AND rg.End_Date IS NULL
	-- AND da.Account_Type_ID in (1, 2) -- Only export bank accounts (Checking, Savings), since we are not importing Credit Card recurring gifts
	AND da.Account_Type_ID = 3 -- Only export credit card
	AND rg.Subscription_ID LIKE 'sub_%' -- only way I can think of to identify stripe gifts vs. pushpay gifts
	AND c.Display_Name != 'Guest Giver'