SELECT
	PageTemplateID,
	PageTemplateCodeName,
	PageTemplateLayout

	FROM 
		CMS_PageTemplate

	WHERE 
		PageTemplateLayout IS NOT NULL 
		AND PageTemplateLayoutID IS NULL