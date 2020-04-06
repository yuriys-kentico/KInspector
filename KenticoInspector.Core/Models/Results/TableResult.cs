using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace KenticoInspector.Core.Models.Results
{
    public class TableResult<T> : Result where T : notnull
    {
        public IEnumerable<T> Rows { get; }

        public override bool HasData => Rows.Any();

        internal TableResult(IEnumerable<T> rows)
        {
            foreach (var row in rows)
            {
                foreach (var property in row.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (property.GetIndexParameters().Length == 0 && property.GetValue(row) is string stringValue)
                    {
                        property.SetValue(row, HttpUtility.HtmlEncode(stringValue));
                    }
                }
            }

            Rows = rows;
        }

        public TableResult<T> WithLabel(Term tableLabel)
        {
            Label = tableLabel;

            return this;
        }
    }
}