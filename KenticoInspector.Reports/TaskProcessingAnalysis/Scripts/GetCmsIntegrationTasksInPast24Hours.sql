SELECT 
    TaskID,
	TaskTitle,
	TaskDataType,
	TaskTime,
	TaskSiteID,
	TaskObjectType,
	TaskObjectID,
	TaskNodeID,
	TaskDocumentID,
	TaskNodeAliasPath
    
FROM 
    Integration_Task

WHERE 
    TaskTime < DATEADD(HOUR, -24, GETDATE())