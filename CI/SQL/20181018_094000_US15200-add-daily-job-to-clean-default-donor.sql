USE [msdb];
GO

DECLARE @job_name VARCHAR(100) = 'Crossroads.Daily';
DECLARE @step_name VARCHAR(100) = 'Move PushPay Donations off of Default Donor';

DECLARE @job_id UNIQUEIDENTIFIER;
SELECT @job_id = job_id FROM sysjobs WHERE [name] = 'Crossroads.Daily';

IF NOT EXISTS (SELECT 1 FROM sysjobsteps WHERE job_id = @job_id AND step_name = @step_name)
BEGIN
    DECLARE @max_job_step_id INT;
    SELECT @max_job_step_id = MAX(step_id) FROM sysjobsteps WHERE job_id = @job_id;

	EXEC sp_update_jobstep
	@step_id = @max_job_step_id,
	@job_name = @job_name,
	@on_success_action = 3,
	@on_fail_action = 3;

    DECLARE @new_job_step_id INT;
    SELECT @new_job_step_id = @max_job_step_id + 1;

	EXEC sp_add_jobstep
    @job_name = @job_name,
	@step_id = @new_job_step_id,
    @step_name = @step_name,
    @subsystem = N'TSQL',
	@cmdexec_success_code = 0,
	@on_success_action = 1,
	@on_success_step_id = 0,
	@on_fail_action = 2,
	@on_fail_step_id = 0,
	@retry_attempts = 0,
	@retry_interval = 0,
	@os_run_priority = 0,
	@command = N'EXEC [dbo].[crds_service_process_unassigned_donations]',
	@database_name = N'MinistryPlatform',
	@flags = 0;
END
GO
