SELECT 
    COUNT(*) 
    
    FROM 
        CMS_WebFarmTask

	WHERE 
        TaskCreated < DATEADD(HOUR, -24, GETDATE())