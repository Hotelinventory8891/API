using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using OfficeOpenXml;

namespace Export2ExcelLibrary
{
    public interface IExcelDifferenceWriter
    {

        #region "Properties"

        ExcelPackage package { get; set; }


        string ExcelFileName
        {
            get;
            set;
        }

        string ExcelFilePath
        {
            get;
            set;
        }


        string Author
        {
            get;
            set;
        }

        string Title
        {
            get;
            set;
        }


        ExcelWorkbook Workbook { get; set; }

        #endregion "Properties"

        bool CreateExcelFromObjectList<T>(DataTable itemList,string TemplateName, bool IsOverride = false, ExcelPackage package = null, IEnumerable<ExcelWorksheet> sheets = null);
    }
}
