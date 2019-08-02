SQL scripts
  Columns indented once, one column per line
  WHERE conditions indented once, one condition per line
  Operations UPPERCASE, columns PascalCase
  A blank line between operations
Data
  Data classes should be under Models\Data
  Data class property types should reasonably match (string, int, DateTime, XDocument)
  Data class should be named {PascalCase table name without underscores} 
    Special cases:
       View_CMS_Tree_Joined: CmsTreeNode
  Sql script should be named Get{DataClassName}{SummaryOfQuery} 
  Data class property names match table columns exactly
  Object names drop Cms for readability, but not properties
Results
  Results classes should be under Models\Results
  Results class named {DataObjectName}Results
  Results class should inherit related data class
  All table results should contain an ID where possible
  Results should not contain ternary operators, instead separate objects should be returned
Metadata
  Kentico applications bolded using **{ApplicationName}**
  Code snippets coded using `{Code}`
  Long description should be a continuation of short description (due to how it appears in the UI)
  All terms sorted alphabetically
  Summary terms should follow [information|good|error|warning]Summary
  Table titles should be under a property called tableNames
  Table titles class should be called TableNameTerms
Code formatting
  Usings sorted with System* first, and with spaces between top-level namespaces
  List initializer starts on new line, and () omitted if not used
  GetResults should generally follow "get SQL data, filter data, call CompileResults, which uses results classes" 
  Each level of LINQ indented one tab from previous line
  use Any() instead of Count() > 0
  ReportResults object initializers should order properties as Type, Status, Summary, Data
  Object initializers in database service or terms should use implicit naming
  Constructors with more than one parameter should be indented
Tests
  Method names follow Should_{Behavior}_When_{Case}
  Test data follows {ObjectName[With|Without]Issue[s]}