SELECT 
    NodeID

    FROM 
        CMS_Tree
	
    WHERE
        NodeParentID NOT IN (
            SELECT 
                NodeID 
            
                FROM 
                    CMS_Tree
        )
		AND NodeParentID != 0