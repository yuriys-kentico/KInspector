SELECT 
    NodeID, 
    NodeAliasPath, 
    NodeSiteID, 
    NodeLinkedNodeID 
FROM 
    CMS_Tree
WHERE
    NodeID NOT IN (
        SELECT 
            DocumentNodeID 
        FROM 
            CMS_Document
    )
	AND NodeLinkedNodeID IS NULL