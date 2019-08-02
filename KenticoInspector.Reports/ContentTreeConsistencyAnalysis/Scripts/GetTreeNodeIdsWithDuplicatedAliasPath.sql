SELECT 
    NodeID 

    FROM 
        CMS_Tree
	
    WHERE 
        NodeAliasPath IN (
		    SELECT 
                NodeAliasPath
			
                FROM 
                    CMS_Tree
			
                GROUP BY 
                    NodeAliasPath

			    HAVING 
                    COUNT(*) > 1
        )