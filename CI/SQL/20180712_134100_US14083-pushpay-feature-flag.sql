-- =============================================
-- Author:      Tim Giblin
-- Create date: 2018-07-12
-- Description:	Create Pushpay Feature Flag Role
-- allows users with role to access Pushpay feature
-- =============================================
USE [MinistryPlatform]
GO
DECLARE @RoleName NVARCHAR(30) = 'Pushpay'
DECLARE @RoleDescription NVARCHAR(255) = 'Ability to access Pushpay features'

INSERT INTO [dbo].[dp_Roles]
            ([Role_Name]
            ,[Description]
            ,[Domain_ID]
            ,[Mass_Email_Quota]
            ,[_AdminRole])
     VALUES
           (@RoleName
            ,@RoleDescription
            ,1
            ,NULL
            ,0)
GO