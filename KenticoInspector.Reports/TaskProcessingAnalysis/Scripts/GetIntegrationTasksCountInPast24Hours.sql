SELECT 
    COUNT(*) 
    
    FROM 
        Integration_Task

	WHERE 
        TaskTime < DATEADD(HOUR, -24, GETDATE())