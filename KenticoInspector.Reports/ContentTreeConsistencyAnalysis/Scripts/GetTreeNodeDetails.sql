SELECT 
    NodeName, 
    NodeAliasPath, 
    NodeID, 
    NodeParentID, 
    NodeSiteID, 
    NodeLevel, 
    NodeClassID, 
    ClassName, 
    ClassDisplayName
FROM 
    View_CMS_Tree_Joined
WHERE 
    NodeID in @nodeIds