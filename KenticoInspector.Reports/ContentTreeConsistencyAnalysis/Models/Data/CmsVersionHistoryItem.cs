using System;
using System.Linq;
using System.Xml.Linq;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Data
{
    public class CmsVersionHistoryItem
    {
        private int _coupledDataId = -1;

        public int VersionHistoryID { get; set; }

        public int DocumentID { get; set; }

        public XDocument NodeXml { get; set; } = null!;

        public int VersionClassID { get; set; }

        public string? VersionDocumentName { get; set; }

        public string DocumentNamePath { get; set; } = null!;

        public DateTime WasPublishedFrom { get; set; }

        public int CoupledDataID
        {
            get
            {
                if (_coupledDataId == -1 && NodeXml != null) _coupledDataId = GetCoupledDataId();

                return _coupledDataId;
            }
        }

        private int GetCoupledDataId()
        {
            var foreignKeyRaw = NodeXml
                .Descendants("DocumentForeignKeyValue")
                .FirstOrDefault()
                ?
                .Value;

            return int.TryParse(
                foreignKeyRaw,
                out var foreignKey
                )
                ? foreignKey
                : -1;
        }
    }
}