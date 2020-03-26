SELECT DISTINCT 
    ClassID,
    NodeSiteID, 
    NodeClassID, 
    ClassName, 
    ClassDisplayName
	
    FROM 
        View_CMS_Tree_Joined
	
    LEFT JOIN CMS_ClassSite 
        ON
		    ClassID = NodeClassID
            AND SiteID = NodeSiteID

	WHERE 
        SiteID IS NULL