using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Export2ExcelLibrary
{
    public class ExcelCell
    {
        public string SourceColumn { get; set; }
        public string DataType { get; set; }
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
        public string WorkBookName { get; set; }
        public string WorkSheetName { get; set; }
        public Nullable<long> SubTemplateId { get; set; }
        public Nullable<long> TemplateId { get; set; }
        public string PropertyName { get; set; }
        public string ColumnKey { get; set; }
        public string TableName { get; set; }
    }
}
