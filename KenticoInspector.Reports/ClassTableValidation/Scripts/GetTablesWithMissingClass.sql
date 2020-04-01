SELECT
	O.name AS TableName
FROM 
    sys.tables O
WHERE 
	O.name COLLATE DATABASE_DEFAULT NOT IN (
		SELECT
			ClassTableName 
		FROM 
			CMS_Class 
		WHERE 
			ClassTableName IS NOT NULL
		)
	AND O.object_id NOT IN (
		SELECT 
            major_id 
		FROM 
			sys.extended_properties 
		WHERE 
			minor_id = 0
			AND class = 1
			AND name = 'microsoft_database_tools_support'
	)