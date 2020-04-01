SELECT 
	DocumentID,
    CAST (NodeXML as XML) NodeXml,
	VersionClassID,
	VersionHistoryID,
    VersionDocumentName,
    DocumentNamePath,
    WasPublishedFrom
FROM 
    CMS_VersionHistory
WHERE 
    VersionHistoryID IN @idsBatch