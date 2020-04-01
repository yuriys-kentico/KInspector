SELECT 
    NodeID
FROM 
    CMS_Tree
WHERE 
    NodeLevel != LEN(NodeAliasPath) - LEN(REPLACE(NodeAliasPath,'/','')) -- NodeLevel = number of '/' in it
    AND NodeLevel != 0 AND NodeParentID != 0 -- Root nodes