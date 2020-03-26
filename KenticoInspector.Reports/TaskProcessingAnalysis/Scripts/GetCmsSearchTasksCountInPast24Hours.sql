SELECT 
    COUNT(*) 
    
    FROM 
        CMS_SearchTask

	WHERE 
        SearchTaskCreated < DATEADD(HOUR, -24, GETDATE())