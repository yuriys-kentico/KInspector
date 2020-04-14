SELECT 
    TaskID,
	TaskType,
	TaskTarget,
	TaskCreated,
	TaskMachineName
    
FROM 
    CMS_WebFarmTask

WHERE 
    TaskCreated < DATEADD(HOUR, -24, GETDATE())