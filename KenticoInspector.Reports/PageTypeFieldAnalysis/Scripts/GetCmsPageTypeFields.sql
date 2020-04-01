﻿SELECT 
    ClassName AS PageTypeCodeName,
    COLUMN_NAME AS FieldName,
    DATA_TYPE AS FieldDataType
FROM
    INFORMATION_SCHEMA.COLUMNS
INNER JOIN CMS_Class ON
    ClassTableName = TABLE_NAME COLLATE DATABASE_DEFAULT
WHERE 
    ClassIsDocumentType = 1