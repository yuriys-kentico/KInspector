SELECT 
	ClassDisplayName,
	CAST (ClassFormDefinition AS XML) ClassFormDefinitionXml,
	ClassID,
	ClassName,
	ClassTableName

    FROM 
        CMS_Class

    WHERE 
        ClassID IN @classIds