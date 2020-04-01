SELECT TOP 25
	O.name AS TableName, 
    MAX(S.row_count) AS Rows,
    SUM(S.reserved_page_count) * 8 / 1024 AS SizeInMB,
	CASE 
		WHEN
			MAX(S.row_count) > 0 
		THEN 
			SUM(S.reserved_page_count) * 8 * 1024 / MAX(S.row_count) 
		ELSE 
			0 
	END AS BytesPerRow
FROM 
    sys.tables O
INNER JOIN sys.dm_db_partition_stats S ON
	S.object_id = O.object_id 
GROUP BY 
    O.name
ORDER BY 
    Rows DESC
