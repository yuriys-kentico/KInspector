SELECT 
    SearchTaskID,
	SearchTaskType,
	SearchTaskObjectType,
	SearchTaskServerName,
	SearchTaskCreated,
	SearchTaskRelatedObjectID
    
FROM 
    CMS_SearchTask

WHERE 
    SearchTaskCreated < DATEADD(HOUR, -24, GETDATE())