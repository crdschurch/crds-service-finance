USE [MinistryPlatform]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		John Cleaver (Ingage)
-- Create date: 08/12/2020
-- Description:	Create raw json table for Donations
-- =============================================

CREATE TABLE [dbo].[cr_RawPushpayDonations](
	[DonationId] [int] IDENTITY(1,1) NOT NULL,
	[RawJson] [nvarchar](max) NOT NULL,
	[IsProcessed] [bit] NOT NULL,
	[TimeCreated] [datetime] NULL,
 CONSTRAINT [PK_cr_RawPushpayDonations] PRIMARY KEY CLUSTERED 
(
	[DonationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[cr_RawPushpayDonations] ADD  CONSTRAINT [DF_cr_RawPushpayDonations_IsProcessed]  DEFAULT ((0)) FOR [IsProcessed]
GO

ALTER TABLE [dbo].[cr_RawPushpayDonations] ADD  CONSTRAINT [df_DonationTimeCreated]  DEFAULT (getdate()) FOR [TimeCreated]
GO

-- -------------------------

USE [MinistryPlatform]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		John Cleaver (Ingage)
-- Create date: 08/12/2020
-- Description:	Insert raw json for Donations
-- =============================================

CREATE   PROCEDURE [dbo].[api_crds_Insert_PushpayDonationsRawJson]
	@RawJson nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
​
	INSERT INTO [dbo].[cr_RawPushpayDonations]
		([RawJson])
	VALUES
		(@RawJson)
END
​
GO


