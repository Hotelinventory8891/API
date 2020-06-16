using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using OfficeOpenXml.Drawing;

namespace Export2ExcelLibrary
{
    public class ExcelObjectWriter : IExcelWriter
    {

        #region "Properties"
        private ExcelPackage _package;
        public ExcelPackage package
        {
            get
            {
                return _package;
            }
            set
            {
                _package = value;
            }
        }

        public string ExcelFileName
        {
            get;
            set;
        }

        public string ExcelFilePath
        {
            get;
            set;
        }


        public string Author
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }


        public ExcelWorkbook Workbook { get; set; }

        #endregion "Properties"
        #region TemplateMethod

        public ExcelObjectWriter()
        {
        }

        public ExcelObjectWriter(string FileName, string Template)
        {
            FileInfo templateFile = new FileInfo(Template);
            if (templateFile.Exists)
            {
                FileInfo rawFile = new FileInfo(FileName);
                if (rawFile.Exists)
                {
                    rawFile.Delete();
                }
                File.Copy(Template, FileName, true);
                this.ExcelFileName = System.IO.Path.GetFileName(FileName);
                this.ExcelFilePath = FileName;
            }
            else
            {
                throw new Exception("Template Not Found");
            }
        }

        bool IExcelWriter.CreateExcelFromObjectList<T>(IList<T> itemList, IList<ExcelCell> mappingList, bool IsOverride = false, ExcelPackage package = null,IEnumerable<ExcelWorksheet> sheets = null) 
        {
            if (string.IsNullOrEmpty(ExcelFilePath)) throw new Exception("Excel file path has not been provided!");
            DirectoryInfo outputDir = new DirectoryInfo(Path.GetDirectoryName(ExcelFilePath));
            if (!outputDir.Exists) throw new Exception("Excel file path does not exist!");

            if (string.IsNullOrEmpty(ExcelFileName)) throw new Exception("Excel file name has not been provided!");
            FileInfo newFile = new FileInfo(outputDir.FullName + @"\" + ExcelFileName);
            if (newFile.Exists)
            {
                if (IsOverride)
                    newFile.Delete();  // create a new file everytime
                newFile = new FileInfo(outputDir.FullName + @"\" + ExcelFileName);
            }

            //var columns = typeof(T).GetProperties().Select(pi => pi.Name).ToArray();

            ExcelPackage pck = package;
            if (package == null)
                pck = new ExcelPackage();

            //set the workbook properties and add a default sheet in it
            //SetWorkbookProperties(pck);

            if (mappingList == null) throw new Exception("Mapping Details Not available!");

            string worksheetName = mappingList.FirstOrDefault().WorkSheetName;

            //Get a sheet
            //if(pck.Workbook.Worksheets.Where(w => w.Name == worksheetName).Count() == 0)
            //     throw new Exception("Sheet not found in the Tamplate");
            //var ws = pck.Workbook.Worksheets.Where(w => w.Name == worksheetName).FirstOrDefault();

            var ws = sheets.Where(s => s.Name == worksheetName).FirstOrDefault();
            if (ws != null)
            {
                //CreateHeader(ws, columns);
                int colIndex = 0;

                int rowIndex = 1;
                PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                //foreach (T dr in itemList) // Adding Data into rows
                //{
                //    colIndex = 1;
                //    rowIndex++;

                //    foreach (PropertyInfo dc in properties)
                //    {
                //        ExcelCell _currentCellprop = mappingList.SingleOrDefault(s => s.PropertyName == dc.Name);
                //        if (_currentCellprop != null)
                //        {
                //            colIndex = _currentCellprop.ColIndex;
                //            if (itemList.Count() < 1)
                //                rowIndex = _currentCellprop.RowIndex;
                //            var cell = ws.Cells[rowIndex, colIndex];
                //            //Setting Value in cell
                //            cell.Value = Convert.ToString(dc.GetValue(dr, null));
                //        }
                //    }
                //}
                rowIndex = mappingList.FirstOrDefault() != null ? mappingList.FirstOrDefault().RowIndex : 1;
                foreach (T dr in itemList) // Adding Data into rows
                {
                    colIndex = 1;
                    foreach (ExcelCell m in mappingList)
                    {

                        PropertyInfo property = properties.Where(x => x.Name == m.PropertyName).FirstOrDefault();
                        if (property != null)
                        {
                            object objValue = property.GetValue(dr, null);
                            if (!string.IsNullOrEmpty(m.DataType) && m.DataType.Equals("DRAWING", StringComparison.OrdinalIgnoreCase))
                            {
                                string imagePath = objValue.ToString();
                                string imageLocation = "6";
                                string imageFormats = "JPG|JPEG|PNG|BMP"; 
                                if (!string.IsNullOrEmpty(imagePath))
                                {
                                    string[] keyColumns = m.ColumnKey.Split(new char[] { ',' });
                                    foreach (var key in keyColumns)
                                    {
                                        if (key.Contains("FORMAT") && key.Contains("="))
                                            imageFormats = key.Split(new char[] { '=' })[1];
                                        if (key.Contains("LOCATION") && key.Contains("="))
                                            imageLocation = key.Split(new char[] { '=' })[1];
                                    }

                                    AttachImage(ws, imagePath, Convert.ToInt32(imageLocation), imageFormats);
                                }
                            }
                            else
                            {
                                if (itemList.Count() < 2)
                                    rowIndex = m.RowIndex;
                                ws.Cells[rowIndex, m.ColIndex].Value = objValue;
                            }
                        }
                    }
                    rowIndex++;
                }

                //foreach (T dr in itemList) // Adding Data into rows
                //{
                //    colIndex = 1;
                //    rowIndex++;

                //    foreach (PropertyInfo dc in properties)
                //    {
                //        ExcelCell _currentCellprop = mappingList.SingleOrDefault(s => s.PropertyName == dc.Name);
                //        if (_currentCellprop != null)
                //        {
                //            colIndex = _currentCellprop.ColIndex;
                //            if(itemList.Count()<1)
                //                rowIndex = _currentCellprop.RowIndex;
                //            var cell = ws.Cells[rowIndex, colIndex];
                //            //Setting Value in cell
                //            cell.Value = Convert.ToString(dc.GetValue(dr, null));
                //        }
                //    }
                //}

                //pck.SaveAs(newFile);
            }
            return true;
        }

        public bool AttachImage(ExcelWorksheet sheet, string path,int imageLocation,string allowedFormats)
        {
            try
            {
                FileInfo fi = new FileInfo (path) ;
                if (fi.Exists  && sheet.Drawings.Count > imageLocation)
                {
                    string[] formats = allowedFormats.Split(new char[] { '|' });
                    if (formats != null)
                    {
                        if (formats.Contains(fi.Extension.ToUpper()))
                        {
                            System.Drawing.Image img = System.Drawing.Image.FromFile(path);
                            (sheet.Drawings[imageLocation] as ExcelPicture).Image = img;
                        }
                    }
                    else
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromFile(path);
                        (sheet.Drawings[imageLocation] as ExcelPicture).Image = img;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);//supress exception  
                return false;
            }
            return true;
        }

        /// <summary>
        /// Set the properties of the Excel file
        /// </summary>
        /// <param name="p"></param>
        private void SetWorkbookProperties(ExcelPackage p)
        {
            //Here setting some document properties
            p.Workbook.Properties.Author = Author;
            p.Workbook.Properties.Title = Title;
        }
        #endregion TemplateMethod




    }
}

