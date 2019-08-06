SELECT 
    COUNT(*) 
    
    FROM 
        Staging_Task

	WHERE 
        TaskTime < DATEADD(HOUR, -24, GETDATE())