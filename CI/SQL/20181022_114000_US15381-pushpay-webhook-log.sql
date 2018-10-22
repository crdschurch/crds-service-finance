USE [MinistryPlatform]
GO

CREATE TABLE dbo.cr_Pushpay_Webhooks (
    Pushpay_Webhook_ID INT identity(1,1) not null,
    Date_Time DATETIME NOT NULL CONSTRAINT [DF_cr_Pushpay_Webhooks_Date_Time] DEFAULT (GETDATE()),
    Event_Type VARCHAR(100) NULL,
    Payload NVARCHAR(MAX) NOT NULL,

    PRIMARY KEY(Pushpay_Webhook_ID)
);

CREATE INDEX IX_cr_Pushpay_Webhooks__DateTime_EventType ON cr_Pushpay_Webhooks(Date_Time, Event_Type);

-- setup permissions so the REST api can access this table
DECLARE @PageID INT = 648;

IF NOT EXISTS(SELECT 1 FROM dp_Pages WHERE Page_ID = @PageID)
BEGIN
    SET IDENTITY_INSERT dbo.dp_Pages ON;
    INSERT INTO dp_Pages (
        Page_ID
        , Display_Name
        , Singular_Name
        , Description
        , View_Order
        , Table_Name
        , Primary_Key
        , Default_Field_List
        , Selected_Record_Expression
        , Display_Copy
    )
    VALUES (
        @PageID
        , 'Pushpay Webhook Logs'
        , 'Pushpay Webhook Log'
        , 'Pushpay Webhook Logs'
        , 300
        , 'cr_Pushpay_Webhooks'
        , 'Pushpay_Webhook_ID'
        , 'Pushpay_Webhook_ID, Date_Time, Event_Type, Payload'
        , 'Pushpay_Webhook_ID'
        , 1
    );
    SET IDENTITY_INSERT dbo.dp_Pages OFF;
END

DECLARE @RoleList TABLE (
    Role_ID INT NOT NULL
);

INSERT INTO @RoleList (Role_ID) VALUES (2), (62), (105), (107), (1023);

INSERT INTO dp_Role_Pages
    (role_id, page_id, access_level, scope_all, Approver, File_Attacher, Data_Importer, Data_Exporter, Secure_Records, Allow_Comments, Quick_Add)
SELECT
    newRoles.Role_ID, @PageID, 3, 0, 0, 1, 0, 1, 1, 0, 1
FROM
    @RoleList newRoles
    LEFT JOIN dp_Role_Pages rp on rp.Page_ID = @PageID AND rp.Role_ID = newRoles.Role_ID
WHERE
    rp.Role_Page_ID IS NULL
;
