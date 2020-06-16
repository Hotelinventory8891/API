using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using OfficeOpenXml;
 

namespace Export2ExcelLibrary
{
    public class ObjectExcelExporter<T>  where T:class,new ()
    {
        private List<PropertyInfo> properties= null;
        private Type type;

        public MemoryStream Export(string TemplateName, List<T> values, List<ExcelCell> templates)
        {
            FileInfo templateFile = new FileInfo(TemplateName);
            MemoryStream ms = null;
            if (templateFile.Exists)
            {
               
                DirectoryInfo outputDir = templateFile.Directory;
                string uniqueName = outputDir.FullName + @"\" + Guid.NewGuid().ToString() + '~' + Path.GetFileName(TemplateName);
                FileInfo newFile = new FileInfo(uniqueName);

                if (newFile.Exists)
                {
                    newFile.Delete();  // create a new file everytime
                    newFile = new FileInfo(uniqueName);
                    File.Copy(TemplateName, uniqueName, true);
                }
                else
                    File.Copy(TemplateName, uniqueName, true);


                type = values.GetType().GetGenericArguments()[0];
                properties = type.GetProperties().ToList();

                if (values != null && values.Count > 0 && templates != null && templates.Count > 0)
                {
                    using (ExcelPackage pkg = new ExcelPackage(new FileInfo(uniqueName)))
                    {
                        ExcelWorkbook workBook = pkg.Workbook;
                        foreach (var record in values)
                            ExportSingleRecord(workBook, record, templates);
                        pkg.Save();
                    }
                }

                ms = new MemoryStream();
                using (FileStream fileStream = new FileStream(newFile.FullName, FileMode.Open, FileAccess.Read))
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    fileStream.CopyTo(ms);
                }
                newFile.Delete();
                ms.Position = 0;
            }
            return ms;
        }

        private void ExportSingleRecord(ExcelWorkbook workBook, T value, List<ExcelCell> template)
        {
            ExcelWorksheet ws = null;
            foreach (var field in template)
            {
                if (ws == null || !ws.Name.Equals(field.WorkSheetName, StringComparison.OrdinalIgnoreCase))
                    ws = workBook.Worksheets[field.WorkSheetName];
                try
                {
                    PropertyInfo property= properties.Find(x => x.Name == field.SourceColumn);
                    if (property!= null)
                    {
                        object objValue = property.GetValue(value, null);
                        ws.Cells[field.RowIndex, field.ColIndex].Value = objValue;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }

}