namespace KenticoInspector.Reports.PageTypeFieldAnalysis.Models.Data
{
    public class CmsPageTypeField
    {
        public string PageTypeCodeName { get; set; } = null!;

        public string? FieldName { get; set; }

        public string? FieldDataType { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is CmsPageTypeField comparingField)
            {
                var fieldsAreEqual = comparingField.FieldName == FieldName &&
                    comparingField.FieldDataType == FieldDataType;

                return fieldsAreEqual;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var hCode = (FieldName + FieldDataType).GetHashCode();

            return hCode;
        }
    }
}