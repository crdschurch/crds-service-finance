/* 
 * Exports credit card recurring gift data to be used
 * for importing recurring schedules into pushpay.
 * There should be an accompanying person file for import
 */
 
 /*
  * Email address is modified so that emails do not
  * get sent out when the result file is imported
  * in PushPay's system
  */

SELECT
	  rg.Subscription_ID AS "Schedule ID" 
	, cast(rg.Amount as decimal(10,2)) AS "Amount" -- Rounding decimals up, it appears PushPay wants whole numbers
	, CASE rgf.Frequency_ID 
		WHEN 1 THEN CONCAT('Every Week ', rd.Day_Of_Week) 
		ELSE CONCAT('Every Month ', CONVERT(varchar, rg.Day_Of_Month)) 
	  END AS "Frequency"
	, FORMAT(rg.Start_Date, 'yyyy-MM-dd') AS "Start Date" -- TODO do we want this?
	, p.Program_Name AS "Fund Name"
	, CASE WHEN Account_Type_ID = 3 THEN 'Credit Card'
		   WHEN Account_Type_ID IN (1,2) THEN 'ACH'
		   ELSE NULL
	   END AS "Method"
	, rg.Subscription_ID AS "Memo" -- this is duplicate, but derrin from pushpay recommends we set this so we can get it back in webhook
	, c.Contact_GUID AS "Person ID" -- TODO only needed if person file
	--, c.Email_Address AS "Email"
	, CONCAT(SUBSTRING(c.Email_Address, 1, CHARINDEX('@', c.Email_Address)-1), '_____', SUBSTRING(c.Email_Address, CHARINDEX('@', c.Email_Address), 1000)) AS "Email"
	, ISNULL(c.Mobile_Phone,'') AS "Mobile Number"
	, c.Nickname AS "First Name"
	, c.Last_Name AS "Last Name"
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
