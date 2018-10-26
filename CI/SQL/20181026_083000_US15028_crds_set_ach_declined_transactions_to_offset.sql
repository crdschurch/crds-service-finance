USE [MinistryPlatform]
GO

----------------------------------------------------------------
--  Pushpay sends an offsetting trnsaction when an ACH donation
--  "fails late" approx. 3-4 business days after the transaction
--  is originally attempted.  The webhook sets the original
--  transaction as declined.  Pushpay's normal integration with
--  MP creates this offsetting transaction which throws off 
--  giving history and statements if we don't set the donation
--  status appropriately
----------------------------------------------------------------

CREATE OR ALTER PROCEDURE [dbo].[crds_set_ach_declined_transactions_to_offset]
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @OffsetDonationStatusId int;

	SELECT @OffsetDonationStatusId = Donation_Status_ID 
	FROM Donation_Statuses
	WHERE Donation_Status = 'offset'

	UPDATE Donations
	SET Donation_Status_ID = @OffsetDonationStatusId
	WHERE Donation_ID IN (
			SELECT Donation_ID
				FROM Donations d
					JOIN Payment_Types pt ON d.Payment_Type_ID = pt.Payment_Type_ID
				WHERE Donation_Amount < 0	
					AND pt.Payment_Type = 'Reimbursement'
					AND Donation_Date > DATEADD(mi, -10, getdate())
			)

END
GO

USE [msdb]
GO

/****** Object:  Job [crds_set_ach_declined_transactions_to_offset]    Script Date: 10/25/2018 1:19:08 PM ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [[Uncategorized (Local)]]    Script Date: 10/25/2018 1:19:08 PM ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'[Uncategorized (Local)]' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'[Uncategorized (Local)]'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'crds_set_ach_declined_transactions_to_offset', 
		@enabled=1, 
		@notify_level_eventlog=2, 
		@notify_level_email=2, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'Execute every 5 minutes: 
sp_executesql crds_set_ach_declined_transactions_to_offset
', 
		@category_name=N'[Uncategorized (Local)]', 
		@owner_login_name=N'sa', 
		@notify_email_operator_name=N'DBA', @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [sp_executesql crds_set_ach_declined_transactions_to_offset]    Script Date: 10/25/2018 1:19:08 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'sp_executesql crds_set_ach_declined_transactions_to_offset', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'sp_executesql crds_set_ach_declined_transactions_to_offset', 
		@database_name=N'MinistryPlatform', 
		@flags=4
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'Every5Minutes', 
		@enabled=1, 
		@freq_type=4, 
		@freq_interval=1, 
		@freq_subday_type=4, 
		@freq_subday_interval=5, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=0, 
		@active_start_date=20181025, 
		@active_end_date=99991231, 
		@active_start_time=0, 
		@active_end_time=235959, 
		@schedule_uid=N'11f5693d-5d7d-4ece-9f6d-f5f3a82d8fce'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave:
GO


