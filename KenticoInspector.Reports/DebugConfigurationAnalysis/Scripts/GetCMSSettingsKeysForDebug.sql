SELECT 
	KeyID,
    KeyName,
    KeyDisplayName,
    KeyValue,
    KeyDefaultValue

    FROM
	    CMS_SettingsKey 

    WHERE
	    KeyName = 'CMSDisableDebug' 
        OR ( 
		    KeyType = 'boolean' AND -- Get only boolean-type settings 
		    KeyName LIKE 'CMSDebug%' AND -- Get only debug-related settings
		    KeyName NOT LIKE '%all%' AND -- Filters out settings to also debug in the admin
		    KeyName NOT LIKE '%live%' AND -- Filters out settings to display debug information on the live site
		    KeyName NOT LIKE '%stack%' AND -- Filters out settings to display stack information
		    KeyName != 'CMSDebugMacrosDetailed' -- Filters out setting for showing more details for macro debugging
	    )