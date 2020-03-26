SELECT 
    Child.NodeID

    FROM 
        CMS_Tree AS Child

    LEFT JOIN CMS_Tree AS Parent 
        ON 
            Child.NodeParentID = Parent.NodeID

	WHERE 
        (Child.NodeLevel - 1) != Parent.NodeLevel