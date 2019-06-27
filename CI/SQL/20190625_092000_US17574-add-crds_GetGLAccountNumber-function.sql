USE [MinistryPlatform]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:      John Cleaver
-- Create date: 6/25/2019
-- Description: Gets GL Account Number by Program Id and Congregation Id. This is to support
-- programmatic creation of Journal Entries.
-- =============================================
CREATE OR ALTER FUNCTION [dbo].[crds_GetGLAccountNumber](@Program_ID INT, @Congregation_ID INT)
RETURNS NVARCHAR(20)
AS
BEGIN
	RETURN 
		(SELECT TOP(1) GL_Account FROM GL_Account_Mapping
		WHERE Program_ID = @Program_ID AND Congregation_ID = @Congregation_ID)
END

GO
