using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Export2ExcelLibrary
{
    public interface IExcelWriter
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

        bool CreateExcelFromObjectList<T>(IList<T> itemList, IList<ExcelCell> mappingList, bool IsOverride = false, ExcelPackage package = null,IEnumerable<ExcelWorksheet> sheets = null);
    }
}
