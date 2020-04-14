SELECT 
    TaskID,
	TaskName,
	TaskEnabled,
	TaskLastRunTime,
	TaskNextRunTime,
	TaskServerName,
	TaskRunIndividually,
	TaskUseExternalService
    
FROM 
    CMS_ScheduledTask

WHERE 
    TaskDeleteAfterLastRun = 1 
	AND TaskNextRunTime < DATEADD(HOUR, -24, GETDATE())