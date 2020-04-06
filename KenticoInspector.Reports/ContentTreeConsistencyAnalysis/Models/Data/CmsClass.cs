using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Data
{
    public class CmsClass
    {
        private IEnumerable<CmsClassField>? _classFields = null;
        private string? _classIdColumn = null;

        public string ClassDisplayName { get; set; } = null!;

        public XDocument? ClassFormDefinitionXml { get; set; }

        public int ClassID { get; set; }

        public string ClassName { get; set; } = null!;

        public string? ClassTableName { get; set; }

        public IEnumerable<CmsClassField>? ClassFields
        {
            get
            {
                if (_classFields == null && ClassFormDefinitionXml != null)
                {
                    _classFields = GetFieldsFromXml();
                }

                return _classFields;
            }
        }

        public string? ClassIDColumn
        {
            get
            {
                if (_classIdColumn == null && ClassFormDefinitionXml != null)
                {
                    _classIdColumn = ClassFields
                        .Where(x => x.IsIdColumn)
                        .Select(x => x.Column)
                        .FirstOrDefault();
                }

                return _classIdColumn;
            }
        }

        private IEnumerable<CmsClassField>? GetFieldsFromXml()
        {
            var fields = ClassFormDefinitionXml?
                .Descendants("field")
                .Select(field =>
                {
                    var isIdColumnRaw = field
                        .Attribute("isPK")?
                        .Value;

                    var isIdColumn = !string.IsNullOrWhiteSpace(isIdColumnRaw) ? bool.Parse(isIdColumnRaw) : false;

                    return new CmsClassField
                    {
                        Caption = field
                            .Descendants("fieldcaption")
                            .FirstOrDefault()?
                            .Value,
                        Column = field
                            .Attribute("column")
                            .Value,
                        ColumnType = field
                            .Attribute("columntype")
                            .Value,
                        DefaultValue = field
                            .Descendants("defaultvalue")
                            .FirstOrDefault()?
                            .Value,
                        IsIdColumn = isIdColumn
                    };
                })
                .ToList();

            return fields;
        }
    }
}