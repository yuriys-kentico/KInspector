SELECT 
    TaskID,
	TaskTitle,
	TaskServers,
	TaskTime,
	TaskSiteID,
	TaskObjectType,
	TaskObjectID,
	TaskNodeID,
	TaskDocumentID,
	TaskNodeAliasPath
    
FROM 
    Staging_Task

WHERE 
    TaskTime < DATEADD(HOUR, -24, GETDATE())