using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Export2ExcelLibrary
{
    public class ExcelObjectDifferenceWriter : IExcelDifferenceWriter
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

        public ExcelObjectDifferenceWriter()
        {
        }

        public ExcelObjectDifferenceWriter(string FileName, string Template)
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


        bool IExcelDifferenceWriter.CreateExcelFromObjectList<T>(DataTable itemList, string TemplateName, bool IsOverride, ExcelPackage package, IEnumerable<ExcelWorksheet> sheets)
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

            //if (mappingList == null) throw new Exception("Mapping Details Not available!");

            //string worksheetName = mappingList.FirstOrDefault().WorkSheetName;
            string worksheetName = TemplateName;// mappingList.FirstOrDefault().WorkSheetName;

            //Get a sheet
            //if(pck.Workbook.Worksheets.Where(w => w.Name == worksheetName).Count() == 0)
            //     throw new Exception("Sheet not found in the Tamplate");
            //var ws = pck.Workbook.Worksheets.Where(w => w.Name == worksheetName).FirstOrDefault();

            var ws = sheets.Where(s => s.Name == worksheetName).FirstOrDefault();
            //CreateHeader(ws, columns);
            int colIndex = 0;

            int rowIndex = 1;
            // PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

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
            //foreach (T dr in itemList) // Adding Data into rows
            //{
            //    colIndex = 1;
            //    rowIndex++;
            //    foreach (ExcelCell m in mappingList)
            //    {

            //        PropertyInfo property = properties.Where(x => x.Name == m.PropertyName).FirstOrDefault();
            //        if (property != null)
            //        {
            //            object objValue = property.GetValue(dr, null);
            //            if (itemList.Count() < 2)
            //                rowIndex = m.RowIndex;
            //            ws.Cells[rowIndex, m.ColIndex].Value = objValue;
            //        }
            //    }
            //}

            //for (int i = 0; i < itemList.Rows.Count; i++)
            //{
            if (ws !=null)
            {
                colIndex = 1;
                rowIndex = 1;
                ////ws.Cells[rowIndex,colIndex].Value = itemList.Rows[rowIndex][colIndex];
                CreateDataDifference(ws, ref rowIndex, itemList);
            }
               
            //}


            //for (int i = 0; i < itemList.Rows.Count; i++)
            //{
            //    DataTable dt = DatasetToExport.Tables[i];
            //    ws = workBook.Worksheets.ElementAt(i); ;
            //    if (ws == null)
            //        ws = CreateSheetDifference(pck, "Sheet" + (i).ToString());////Create a sheet
            //    int rowIndex = 1;
            //    if (ws != null)
            //    {
            //        CreateDataDifference(ws, ref rowIndex, dt, Template);
            //    }
            //}

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



            return true;
        }

        //private void CreateDataDifference(ExcelWorksheet ws, ref int rowIndex, DataTable dt)
        //{
        //    if (dt != null && dt.Rows.Count > 0)
        //    {

        //       int rowCell = 0;

        //        for (int row = 0; row < dt.Rows.Count - 1; row = row + 2)
        //        {

        //            for (int col = 0; col < dt.Columns.Count - 1; col++)
        //            {
        //                //if (dt.Rows[row][0].Equals(dt.Rows[row + 1][0]))
        //                //{

        //                if (dt.Rows[row][col].Equals(dt.Rows[row + 1][col]))
        //                {
        //                    var cell = ws.Cells[rowCell + 2, col + 1];
        //                    string typeString = dt.Columns[col].DataType.ToString();// dc.DataType.ToString();
        //                    if (typeString == "System.Int32" || typeString == "System.Int64" || typeString == "System.Double")
        //                    {
        //                        cell.Value = dt.Rows[row][col];// dr[dc.ColumnName];
        //                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    }
        //                    else
        //                    {
        //                        //Setting Value in cell
        //                        cell.Value = Convert.ToString(dt.Rows[row][col]);//dr[dc.ColumnName]);
        //                    }

        //                    var cell2 = ws.Cells[rowCell + 3, col + 1];
        //                    cell2.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    cell2.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
        //                    string typeString2 = dt.Columns[col].DataType.ToString();// dc.DataType.ToString();
        //                    if (typeString == "System.Int32" || typeString == "System.Int64" || typeString == "System.Double")
        //                    {
        //                        cell2.Value = dt.Rows[row + 1][col];// dr[dc.ColumnName];
        //                        cell2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    }
        //                    else
        //                    {
        //                        //Setting Value in cell
        //                        cell2.Value = Convert.ToString(dt.Rows[row + 1][col]);//dr[dc.ColumnName]);
        //                    }
        //                }
        //                else
        //                {

        //                    var cell = ws.Cells[rowCell + 2, col + 1];
        //                    cell.Style.Font.Color.SetColor(Color.Red);
        //                    string typeString = dt.Columns[col].DataType.ToString();// dc.DataType.ToString();
        //                    if (typeString == "System.Int32" || typeString == "System.Int64" || typeString == "System.Double")
        //                    {
        //                        cell.Value = dt.Rows[row][col];// dr[dc.ColumnName];
        //                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

        //                    }
        //                    else
        //                    {
        //                        //Setting Value in cell
        //                        cell.Value = Convert.ToString(dt.Rows[row][col]);//dr[dc.ColumnName]);
        //                    }
        //                    var cell2 = ws.Cells[rowCell + 3, col + 1];
        //                    cell2.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    cell2.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
        //                    string typeString2 = dt.Columns[col].DataType.ToString();// dc.DataType.ToString();
        //                    if (typeString == "System.Int32" || typeString == "System.Int64" || typeString == "System.Double")
        //                    {
        //                        cell2.Value = dt.Rows[row + 1][col];// dr[dc.ColumnName];
        //                        cell2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                        //cell.Style.Fill.BackgroundColor = new ExcelColor("Red")
        //                    }
        //                    else
        //                    {
        //                        //Setting Value in cell
        //                        cell2.Value = Convert.ToString(dt.Rows[row + 1][col]);//dr[dc.ColumnName]);
        //                    }

        //                }
        //                //}
        //            }

        //            rowCell += 2;

        //        }
        //    }
        //}

        private void CreateDataDifference(ExcelWorksheet ws, ref int rowIndex, DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 1)
            {
                int rowCell = 0;

                for (int row = 0; row < dt.Rows.Count - 1; row = row + 2)
                {

                    for (int col = 0; col < dt.Columns.Count - 1; col++)
                    {

                        if (dt.Rows[row][col].Equals(dt.Rows[row + 1][col]))
                        {
                            var cell = ws.Cells[rowCell + 2, col + 1];
                            string typeString = dt.Columns[col].DataType.ToString();// dc.DataType.ToString();
                            if (typeString == "System.Int32" || typeString == "System.Int64" || typeString == "System.Double")
                            {
                                cell.Value = dt.Rows[row][col];// dr[dc.ColumnName];
                                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            }
                            else
                            {
                                //Setting Value in cell
                                cell.Value = Convert.ToString(dt.Rows[row][col]);//dr[dc.ColumnName]);
                            }

                            var cell2 = ws.Cells[rowCell + 3, col + 1];
                            cell2.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cell2.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            string typeString2 = dt.Columns[col].DataType.ToString();// dc.DataType.ToString();
                            if (typeString == "System.Int32" || typeString == "System.Int64" || typeString == "System.Double")
                            {
                                cell2.Value = dt.Rows[row + 1][col];// dr[dc.ColumnName];
                                cell2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            }
                            else
                            {
                                //Setting Value in cell
                                cell2.Value = Convert.ToString(dt.Rows[row + 1][col]);//dr[dc.ColumnName]);
                            }
                        }
                        else
                        {

                            var cell = ws.Cells[rowCell + 2, col + 1];
                            cell.Style.Font.Color.SetColor(Color.Red);
                            string typeString = dt.Columns[col].DataType.ToString();// dc.DataType.ToString();
                            if (typeString == "System.Int32" || typeString == "System.Int64" || typeString == "System.Double")
                            {
                                cell.Value = dt.Rows[row][col];// dr[dc.ColumnName];
                                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            }
                            else
                            {
                                //Setting Value in cell
                                cell.Value = Convert.ToString(dt.Rows[row][col]);//dr[dc.ColumnName]);
                            }
                            var cell2 = ws.Cells[rowCell + 3, col + 1];
                            cell2.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cell2.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            string typeString2 = dt.Columns[col].DataType.ToString();// dc.DataType.ToString();
                            if (typeString == "System.Int32" || typeString == "System.Int64" || typeString == "System.Double")
                            {
                                cell2.Value = dt.Rows[row + 1][col];// dr[dc.ColumnName];
                                cell2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                //cell.Style.Fill.BackgroundColor = new ExcelColor("Red")
                            }
                            else
                            {
                                //Setting Value in cell
                                cell2.Value = Convert.ToString(dt.Rows[row + 1][col]);//dr[dc.ColumnName]);
                            }

                        }
                        //}
                    }

                    rowCell += 2;

                }
            }
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



        //ExcelPackage IExcelDifferenceWriter.package
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //string IExcelDifferenceWriter.ExcelFileName
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //string IExcelDifferenceWriter.ExcelFilePath
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //string IExcelDifferenceWriter.Author
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //string IExcelDifferenceWriter.Title
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //ExcelWorkbook IExcelDifferenceWriter.Workbook
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}


    }
}
