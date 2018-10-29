USE [MinistryPlatform]
GO

-- Processes PushPay donations that could not be matched to a specific donor.
-- When PushPay cannot find a matching Donor, it assigns the donation to the
-- Default Donor.  We want to move donations that are assigned to Default Donor
-- (either assign to existing donor if we can find a good match, or create a new
-- donor if there isn't an existing donor that's a good match).  Matching will
-- be based on Last Name, Email Address, Phone Number, and the first 2 characters
-- of the First Name.
CREATE OR ALTER PROCEDURE [dbo].[crds_service_process_unassigned_donations]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Audit_Records dbo.crds_Audit_Item;

    DECLARE @DefaultDonorID INT = 1;
    DECLARE @DomainID INT = 1;

    -- ====== 1: Get all donations and recurring gifts assigned to the Default Donor, and parse notes

    -- This table variable stores data for both Donations and Recurring_Gifts.  Donations will have a
    -- non-null Donation_ID, and recurring gifts will have a non-null Recurring_Gift_ID.
    DECLARE @UnassignedData TABLE (
        Donation_ID INT,
        Recurring_Gift_ID INT,
        Contact_ID INT,
        Donor_ID INT,
        First_Name NVARCHAR(50),
        Last_Name VARCHAR(50),
        Phone_Number NVARCHAR(25),
        Email_Address VARCHAR(255)
    );

    INSERT INTO @UnassignedData
        (Donation_ID, Recurring_Gift_ID, Contact_ID, Donor_ID, First_Name, Last_Name, Phone_Number, Email_Address)
    SELECT
        Donation_ID,
        Recurring_Gift_ID,
        NULL,
        NULL,
        First_Name = CASE WHEN LNS-FNS > 0 THEN LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(SUBSTRING(Notes,FNS,LNS-FNS),'First Name:',''),Char(10),''),Char(13),''))) END
        ,Last_Name = CASE WHEN PHS-LNS > 0 THEN LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(SUBSTRING(Notes,LNS,PHS-LNS),'Last Name:',''),Char(10),''),Char(13),''))) END
        ,Phone_Number = CASE WHEN EMS-PHS > 0 THEN LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(SUBSTRING(Notes,PHS,EMS-PHS),'Phone:',''),Char(10),''),Char(13),''))) END
        ,Email_Address = CASE WHEN A1S-EMS > 0 THEN LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(SUBSTRING(Notes,EMS,A1S-EMS),'Email:',''),Char(10),''),Char(13),''))) END
    FROM
        (
            SELECT
                Donation_ID,
                Recurring_Gift_ID = NULL,
                CharIndex('First Name:', Donations.Notes) AS FNS
                ,CharIndex('Last Name:', Donations.Notes) AS LNS
                ,CharIndex('Phone:', Donations.Notes) AS PHS
                ,CharIndex('Email:', Donations.Notes) AS EMS
                ,CharIndex('Address1:', Donations.Notes) AS A1S
                ,CharIndex('Address2:', Donations.Notes) AS A2S	
                ,CharIndex('City, State Zip:', Donations.Notes) AS CSZS
                ,Notes
            FROM
                Donations
            WHERE
                Donor_ID = @DefaultDonorID
            UNION
            SELECT
                Donation_ID = NULL,
                Recurring_Gift_ID,
                CharIndex('First Name:', Recurring_Gifts.Notes) AS FNS
                ,CharIndex('Last Name:', Recurring_Gifts.Notes) AS LNS
                ,CharIndex('Phone:', Recurring_Gifts.Notes) AS PHS
                ,CharIndex('Email:', Recurring_Gifts.Notes) AS EMS
                ,CharIndex('Address1:', Recurring_Gifts.Notes) AS A1S
                ,CharIndex('Address2:', Recurring_Gifts.Notes) AS A2S	
                ,CharIndex('City, State Zip:', Recurring_Gifts.Notes) AS CSZS
                ,Notes
            FROM
                Recurring_Gifts
            WHERE
                Donor_ID = @DefaultDonorID
        ) parsed
    ;

    -- convert phone numbers from "(xxx) xxx-xxxx" to "xxx-xxx-xxxx" 
    UPDATE @UnassignedData SET Phone_Number = REPLACE(REPLACE(Phone_Number, '(', ''), ') ', '-');

    -- ====== 2: Try to find matching Contacts for each donation and recurring gift
    UPDATE ud
    SET
        ud.Contact_ID = c.Contact_ID,
        ud.Donor_ID = c.Donor_Record
    FROM
        @UnassignedData ud
        INNER JOIN Contacts c ON (
            c.Last_Name = ud.Last_Name
            AND c.Mobile_Phone = ud.Phone_Number
            AND c.Email_Address = ud.Email_Address
            AND SUBSTRING(COALESCE(c.First_Name, ''), 1, 2) = SUBSTRING(COALESCE(ud.First_Name, ''), 1, 2)   -- first 2 chars
        )
    WHERE
        -- we must match exactly one Contact (i.e., ignore if multiple Contacts match)
        (SELECT COUNT(*) FROM Contacts c WHERE
            c.Last_Name = ud.Last_Name
            AND c.Mobile_Phone = ud.Phone_Number
            AND c.Email_Address = ud.Email_Address
            AND SUBSTRING(COALESCE(c.First_Name, ''), 1, 2) = SUBSTRING(COALESCE(ud.First_Name, ''), 1, 2)   -- first 2 chars
        ) = 1
    ;

    -- ====== 3: Create new Contacts where we didn't match an existing
    BEGIN TRAN

    BEGIN TRY
        DECLARE @ContactsAdded TABLE (
            Contact_ID INT NOT NULL,
            First_Name NVARCHAR(50),
            Last_Name VARCHAR(50),
            Phone_Number NVARCHAR(25),
            Email_Address VARCHAR(255)
        );

        INSERT INTO Contacts
            (Company, Display_Name, First_Name, Last_Name, Nickname, Email_Address, Mobile_Phone, Household_Position_ID, Domain_ID)
        OUTPUT 
            INSERTED.Contact_ID, INSERTED.First_Name, INSERTED.Last_Name, INSERTED.Mobile_Phone, INSERTED.Email_Address INTO @ContactsAdded
        SELECT
            0,
            CONCAT(Last_Name, ', ', First_Name),
            First_Name,
            Last_Name,
            First_Name,
            Email_Address,
            Phone_Number,
            1,  -- Head of Household
            @DomainID
        FROM
            (
                SELECT DISTINCT
                    First_Name,
                    Last_Name,
                    Email_Address,
                    Phone_Number
                FROM
                    @UnassignedData
                WHERE
                    Contact_ID IS NULL
                    AND First_Name NOT LIKE '%[ /&]%'  -- exclude " and ", "&", " ", "/"
            ) AS x
        ;

        -- capture the IDs for the new Contacts we just created
        UPDATE ud
        SET
            ud.Contact_ID = c.Contact_ID
        FROM
            @UnassignedData ud
            INNER JOIN @ContactsAdded c ON (
                c.Last_Name = ud.Last_Name
                AND c.Phone_Number = ud.Phone_Number
                AND c.Email_Address = ud.Email_Address
                AND c.First_Name = ud.First_Name
            )
        ;

        -- Audit entries for Contacts we created
        INSERT INTO @Audit_Records
            (Table_Name, Record_ID, Audit_Description)
        SELECT
            'Contacts', Contact_ID, 'Created'
        FROM
            @ContactsAdded
        ORDER BY
            Contact_ID
        ;

        -- ====== 4: Create Participant records for new Contacts we just created
        DECLARE @ParticipantsAudit TABLE (
            Participant_ID INT NOT NULL,
            Contact_ID INT NOT NULL
        );

        INSERT INTO Participants
            (Contact_ID, Participant_Type_ID, Participant_Start_Date, Domain_ID)
        OUTPUT INSERTED.Participant_ID, INSERTED.Contact_ID INTO @ParticipantsAudit
        SELECT
            Contact_ID,
            1,      -- *Temp Participant Type
            GETDATE(),
            @DomainID
        FROM
            @ContactsAdded
        ORDER BY
            Contact_ID
        ;

        -- Audit entries for Participants we created
        INSERT INTO @Audit_Records
            (Table_Name, Record_ID, Audit_Description)
        SELECT
            'Participants', Participant_ID, 'Created'
        FROM
            @ParticipantsAudit
        ORDER BY
            Participant_ID
        ;

        -- link Contact to new Participant record
        UPDATE c
        SET c.Participant_Record = p.Participant_ID
        FROM
            Contacts c
            INNER JOIN @ParticipantsAudit p ON p.Contact_ID = c.Contact_ID
        WHERE
            c.Participant_Record IS NULL    -- this check isn't strictly necessary, but included as a precaution
        ;

        -- ====== 5: Create Donor records for new or existing Contacts who don't already have a Donor record
        DECLARE @SetupDate DATETIME = GETDATE();

        DECLARE @DonorsAdded TABLE (
            Contact_ID int not null,
            Donor_ID int not null
        );

        INSERT INTO Donors
            (Contact_ID, Statement_Frequency_ID, Statement_Type_ID, Statement_Method_ID, Setup_Date, Domain_ID)
        OUTPUT INSERTED.Contact_ID, INSERTED.Donor_ID INTO @DonorsAdded
        SELECT DISTINCT
            Contact_ID,
            Statement_Frequency_ID = 1,     -- Quarterly
            Statement_Type_ID = 1,          -- Individual
            Statement_Method_ID = 2,        -- Email/Online
            @SetupDate,
            @DomainID
        FROM
            @UnassignedData
        WHERE
            Contact_ID IS NOT NULL
            AND Donor_ID IS NULL
        ;

        -- capture the IDs for the new Donors we just created
        UPDATE ud
        SET ud.Donor_ID = newDonors.Donor_ID
        FROM
            @UnassignedData ud
            INNER JOIN @DonorsAdded newDonors ON newDonors.Contact_ID = ud.Contact_ID
        ;

        -- Audit entries for Donors we created
        INSERT INTO @Audit_Records
            (Table_Name, Record_ID, Audit_Description)
        SELECT
            'Donors', Donor_ID, 'Created'
        FROM
            @DonorsAdded
        ORDER BY
            Donor_ID
        ;

        -- link Contact to new Donor record
        DECLARE @ContactDonorAudit TABLE (
            Contact_ID int not null,
            New_Donor_ID int
        );

        UPDATE c
        SET c.Donor_Record = newDonors.Donor_ID
        OUTPUT INSERTED.Contact_ID, INSERTED.Donor_Record INTO @ContactDonorAudit
        FROM
            Contacts c
            INNER JOIN @DonorsAdded newDonors ON newDonors.Contact_ID = c.Contact_ID
        WHERE
            c.Donor_Record IS NULL       -- this check isn't strictly necessary, but included as a precaution
        ;

        -- Audit entries for Contacts that we linked with new Donor records
        INSERT INTO @Audit_Records
            (Table_Name, Record_ID, Audit_Description, Field_Name, Field_Label, New_Value, New_ID)
        SELECT
            'Contacts', cda.Contact_ID, 'Updated', 'Donor_Record', 'Donor Record', CONCAT(c.Display_Name, '; ', c.Email_Address), cda.New_Donor_ID
        FROM
            @ContactDonorAudit cda
            LEFT JOIN Contacts c ON c.Contact_ID = cda.Contact_ID
        ORDER BY
            cda.Contact_ID
        ;

        -- ====== 6: Move donations and recurring gifts from default donor to new donor

        -- Donations and Recurring_Gifts both link to Donor_Accounts, which link to Donors.  Adjust
        -- the Donor_ID on related Donor_Accounts first.
        DECLARE @DonorAccountAudit TABLE (
            Donor_Account_ID INT NOT NULL,
            Old_Donor_ID INT,
            New_Donor_ID INT
        );

        UPDATE da
        SET da.Donor_ID = ud.Donor_ID
        OUTPUT INSERTED.Donor_Account_ID, DELETED.Donor_ID, INSERTED.Donor_ID INTO @DonorAccountAudit
        FROM
            @UnassignedData ud
            INNER JOIN Donations d ON d.Donation_ID = ud.Donation_ID
            INNER JOIN Donor_Accounts da ON da.Donor_Account_ID = d.Donor_Account_ID
        WHERE
            ud.Donor_ID IS NOT NULL
            AND da.Donor_ID = @DefaultDonorID
        ;

        UPDATE da
        SET da.Donor_ID = ud.Donor_ID
        OUTPUT INSERTED.Donor_Account_ID, DELETED.Donor_ID, INSERTED.Donor_ID INTO @DonorAccountAudit
        FROM
            @UnassignedData ud
            INNER JOIN Recurring_Gifts rg ON rg.Recurring_Gift_ID = ud.Recurring_Gift_ID
            INNER JOIN Donor_Accounts da ON da.Donor_Account_ID = rg.Donor_Account_ID
        WHERE
            ud.Donor_ID IS NOT NULL
            AND da.Donor_ID = @DefaultDonorID
        ;

        -- Audit entries for Donor_ID change on Donor_Accounts
        INSERT INTO @Audit_Records
            (Table_Name, Record_ID, Audit_Description, Field_Name, Field_Label, New_Value, New_ID, Previous_Value, Previous_ID)
        SELECT
            'Donor_Accounts',
            daa.Donor_Account_ID,
            'Updated',
            'Donor_ID',
            'Donor',
            CONCAT(c2.Display_Name, '; ', c2.Email_Address),
            daa.New_Donor_ID,
            CONCAT(c1.Display_Name, '; ', c1.Email_Address),
            daa.Old_Donor_ID
        FROM
            @DonorAccountAudit daa
            LEFT JOIN Contacts c1 ON c1.Donor_Record = daa.Old_Donor_ID
            LEFT JOIN Contacts c2 ON c2.Donor_Record = daa.New_Donor_ID
        ORDER BY
            daa.Donor_Account_ID
        ;

        -- reassign donations from default donor to new donor
        DECLARE @DonationAudit TABLE (
            Donation_ID INT NOT NULL,
            Old_Donor_ID INT,
            New_Donor_ID INT
        );

        UPDATE d
        SET d.Donor_ID = ud.Donor_ID
        OUTPUT INSERTED.Donation_ID, DELETED.Donor_ID, INSERTED.Donor_ID INTO @DonationAudit
        FROM
            @UnassignedData ud
            INNER JOIN Donations d ON d.Donation_ID = ud.Donation_ID
        WHERE
            ud.Donor_ID IS NOT NULL
            AND d.Donor_ID = @DefaultDonorID
        ;

        -- Audit entries for Donor_ID change on Donations
        INSERT INTO @Audit_Records
            (Table_Name, Record_ID, Audit_Description, Field_Name, Field_Label, New_Value, New_ID, Previous_Value, Previous_ID)
        SELECT
            'Donations',
            da.Donation_ID,
            'Updated',
            'Donor_ID',
            'Donor',
            CONCAT(c2.Display_Name, '; ', c2.Email_Address),
            da.New_Donor_ID,
            CONCAT(c1.Display_Name, '; ', c1.Email_Address),
            da.Old_Donor_ID
        FROM
            @DonationAudit da
            LEFT JOIN Contacts c1 ON c1.Donor_Record = da.Old_Donor_ID
            LEFT JOIN Contacts c2 ON c2.Donor_Record = da.New_Donor_ID
        ORDER BY
            da.Donation_ID
        ;

        -- reassign recurring gifts from default donor to new donor
        DECLARE @RecurringGiftAudit TABLE (
            Recurring_Gift_ID INT NOT NULL,
            Old_Donor_ID INT,
            New_Donor_ID INT
        );

        UPDATE rg
        SET rg.Donor_ID = ud.Donor_ID
        OUTPUT INSERTED.Recurring_Gift_ID, DELETED.Donor_ID, INSERTED.Donor_ID INTO @RecurringGiftAudit
        FROM
            @UnassignedData ud
            INNER JOIN Recurring_Gifts rg ON rg.Recurring_Gift_ID = ud.Recurring_Gift_ID
        WHERE
            ud.Donor_ID IS NOT NULL
            AND rg.Donor_ID = @DefaultDonorID
        ;

        -- Audit entries for Donor_ID change on Recurring_Gifts
        INSERT INTO @Audit_Records
            (Table_Name, Record_ID, Audit_Description, Field_Name, Field_Label, New_Value, New_ID, Previous_Value, Previous_ID)
        SELECT
            'Recurring_Gifts',
            ra.Recurring_Gift_ID,
            'Updated',
            'Donor_ID',
            'Donor',
            CONCAT(c2.Display_Name, '; ', c2.Email_Address),
            ra.New_Donor_ID,
            CONCAT(c1.Display_Name, '; ', c1.Email_Address),
            ra.Old_Donor_ID
        FROM
            @RecurringGiftAudit ra
            LEFT JOIN Contacts c1 ON c1.Donor_Record = ra.Old_Donor_ID
            LEFT JOIN Contacts c2 ON c2.Donor_Record = ra.New_Donor_ID
        ORDER BY
            ra.Recurring_Gift_ID
        ;

        -- Write all audit records
        DECLARE @AuditDate DATETIME = GETDATE();
        EXEC crds_Add_Audit_Items @Audit_Records, @AuditDate, 'Svc Mngr', 0;

        COMMIT TRAN

        DECLARE @NumDonations INT;
        SELECT @NumDonations = COUNT(*) FROM @DonationAudit;

        DECLARE @NumRecurringGifts INT;
        SELECT @NumRecurringGifts = COUNT(*) FROM @RecurringGiftAudit;
        PRINT 'crds_service_process_unassigned_donations: updated '
            + CONVERT(VARCHAR(20), @NumDonations) + ' donations and '
            + CONVERT(VARCHAR(20), @NumRecurringGifts) + ' recurring gifts';
    END TRY

    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRAN
        PRINT 'crds_service_process_unassigned_donations failed: ' + COALESCE(ERROR_MESSAGE(), '');
    END CATCH    
END
GO
