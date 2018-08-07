-- People File
SELECT DISTINCT
	  c.Donor_Record AS "Person ID"
	, CASE WHEN c.Nickname IS NULL OR LEN(LTRIM(RTRIM(c.Nickname))) = 0 THEN c.First_Name ELSE c.Nickname END AS "First Name"
	, c.Last_Name AS "Last Name"
	, COALESCE(c.Email_Address, '') AS "Email"
	, LTRIM(RTRIM(COALESCE(c.Mobile_Phone, ''))) AS "Mobile Number"
	, COALESCE(a.Address_Line_1, '') AS "Address 1"
	, LTRIM(RTRIM(COALESCE(a.Address_Line_2, ''))) AS "Address 2"
	, COALESCE(a.City, '') AS "City"
	, COALESCE(a.[State/Region], '') AS "State"
	, COALESCE(a.Postal_Code, '') AS "Zip"
	, LTRIM(RTRIM(COALESCE(a.Country_Code, ''))) AS "Country"
	, CASE WHEN c.Date_of_Birth IS NULL THEN '' ELSE FORMAT(c.Date_of_Birth, 'yyyy-MM-dd') END AS "DOB"
	, '' AS "Gender" -- Not setting gender for now
FROM
	[MinistryPlatform].[dbo].[Contacts] c
	LEFT OUTER JOIN [MinistryPlatform].[dbo].[Households] h ON h.Household_ID = c.Household_ID
	LEFT OUTER JOIN [MinistryPlatform].[dbo].[Addresses] a ON a.Address_ID = h.Address_ID
	JOIN [MinistryPlatform].[dbo].[Donations] d ON d.Donor_ID = c.Donor_Record
	JOIN [MinistryPlatform].[dbo].[Donation_Distributions] dd ON dd.Donation_ID = d.Donation_ID
	JOIN [MinistryPlatform].[dbo].[Programs] p ON p.Program_ID = dd.Program_ID
WHERE
	d.Payment_Type_ID IN (1, 4, 5)
	AND d.Donation_Date >= '2017-01-01';