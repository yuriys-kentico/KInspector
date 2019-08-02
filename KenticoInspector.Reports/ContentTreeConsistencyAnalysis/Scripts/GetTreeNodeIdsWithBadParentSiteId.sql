﻿SELECT 
    NodeID

    FROM 
        CMS_Tree 
	
    WHERE
		NodeSiteID IS NULL
		OR NodeSiteID = 0
		OR NodeSiteID NOT IN (
            SELECT 
                SiteID 
            
                FROM 
                    CMS_Site
        )