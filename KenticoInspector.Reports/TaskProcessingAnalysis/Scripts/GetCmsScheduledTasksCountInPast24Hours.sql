﻿SELECT 
    COUNT(*) 
    
    FROM 
        CMS_ScheduledTask

	WHERE 
        TaskDeleteAfterLastRun = 1 
	    AND TaskNextRunTime < DATEADD(HOUR, -24, GETDATE())