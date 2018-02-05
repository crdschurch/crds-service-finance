USE [MinistryPlatform]
GO

BEGIN
    SET IDENTITY_INSERT [dbo].[Recurring_Gift_Frequencies] ON;
    
    IF NOT EXISTS(SELECT * FROM Recurring_Gift_Frequencies WHERE Frequency_ID = 3)
        BEGIN
            INSERT INTO [dbo].[Recurring_Gift_Frequencies]([Frequency_ID],[Frequency],[Domain_ID]) VALUES(3,'1stAnd15th',1);
        END

    IF NOT EXISTS(SELECT * FROM Recurring_Gift_Frequencies WHERE Frequency_ID = 4)
        BEGIN    
            INSERT INTO [dbo].[Recurring_Gift_Frequencies]([Frequency_ID],[Frequency],[Domain_ID]) VALUES(4,'OtherWeek',1);
        END

    SET IDENTITY_INSERT [dbo].[Recurring_Gift_Frequencies] OFF;
END