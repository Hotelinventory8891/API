#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Data;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Reflection;

#endregion Using

/// <remarks>
/// <c>Class:</c>Export2ExcelLib
/// <c>Description:</c>Export Data to Excel sheet.
/// <c>Date:</c> 08/09/2012
/// <c>Author:</c> Sumit Saha Roy
/// <c>Copyright (c) 2011 Ericsson. All rights reserved.</c>
/// </remarks>

public class Export2ExcelLib
{

    #region "Properties"

    private string _excelFileName;
    private string _excelFilePath;
    private DataSet _datasetToExport;
    private DataTable _datatableToExport;

    private string _author;
    private string _title;

    /// <summary>
    /// Excel File Name to save as
    /// </summary>
    public string ExcelFileName
    {
        get { return _excelFileName; }
        set { _excelFileName = value; }
    }

    /// <summary>
    /// Path to save the Excel file
    /// </summary>
    public string ExcelFilePath
    {
        get { return _excelFilePath; }
        set { _excelFilePath = value; }
    }

    /// <summary>
    /// Dataset to get the data, in case of exporting from Dataset
    /// </summary>
    public DataSet DatasetToExport
    {
        get { return _datasetToExport; }
        set { _datasetToExport = value; }
    }

    /// <summary>
    /// DataTable to get the data, in case of exporting from datatable
    /// </summary>
    public DataTable DatatableToExport
    {
        get { return _datatableToExport; }
        set { _datatableToExport = value; }
    }

    /// <summary>
    /// Set the Author of the Excel file
    /// </summary>
    public string Author
    {
        get { return _author; }
        set { _author = value; }
    }

    /// <summary>
    /// Set the Title of the Excel file
    /// </summary>
    public string Title
    {
        get { return _title; }
        set { _title = value; }
    }

    /// <summary>
    /// File stream to generate excel file
    /// </summary>
    public Stream FileStream
    {
        get;
        set;
    }

    public ExcelWorkbook Workbook { get; set; }
    public XDocument XmlSchema { get; set; }

    /// <summary>
    /// Dataset to get the data and exporting from Dataset
    /// </summary>
    public DataSet DatasetToExportDifference
    {
        get { return _datasetToExport; }
        set { _datasetToExport = value; }
    }
    #endregion "Properties"


    #region "Public Methods"

    /// <summary>
    /// Export Data from dataset to Excel sheet(s). It will create as many sheets as many tables are there in the dataset
    /// </summary>
    /// <returns></returns>
    public bool CreateExcelFromDataset()
    {
        FileInfo newFile = null;
        if (FileStream == null)
        {
            try
            {
                if (string.IsNullOrEmpty(ExcelFilePath)) throw new Exception("Excel file path has not been provided!");
                DirectoryInfo outputDir = new DirectoryInfo(ExcelFilePath);
                if (!outputDir.Exists) throw new Exception("Excel file path does not exist!");

                if (string.IsNullOrEmpty(ExcelFileName)) throw new Exception("Excel file name has not been provided!");
                newFile = new FileInfo(outputDir.FullName + @"\" + ExcelFileName);
                if (newFile.Exists)
                {
                    newFile.Delete();  // create a new file everytime
                    newFile = new FileInfo(outputDir.FullName + @"\" + ExcelFileName);
                }
            }
            catch (Exception)
            {
                //log it.
            }
        }

        try
        {
            if (this.DatasetToExport == null) throw new Exception("No data to export!");
            if (this.DatasetToExport != null)
            {
                using (ExcelPackage pck = new ExcelPackage())
                {
                    //set the workbook properties and add a default sheet in it
                    SetWorkbookProperties(pck);

                    foreach (DataTable dt in this.DatasetToExport.Tables)
                    {
                        //Create a sheet
                        var ws = pck.Workbook.Worksheets.Add(dt.TableName);

                        //row index of the excel sheet
                        int rowIndex = 1;

                        CreateHeader(ws, ref rowIndex, dt);
                        CreateData(ws, ref rowIndex, dt);
                    }

                    if (newFile != null)
                        pck.SaveAs(newFile);
                    else if (FileStream != null)
                    {
                        using (FileStream)
                        {
                            pck.SaveAs(this.FileStream);
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            //log it
            throw;
        }
        return true;
    }

    /// <summary>
    /// Export Data from datatable to Excel sheet
    /// </summary>
    /// <returns></returns>
    public bool CreateExcelFromDataTable()
    {
        if (string.IsNullOrEmpty(ExcelFilePath)) throw new Exception("Excel file path has not been provided!");
        DirectoryInfo outputDir = new DirectoryInfo(ExcelFilePath);
        if (!outputDir.Exists) throw new Exception("Excel file path does not exist!");

        if (string.IsNullOrEmpty(ExcelFileName)) throw new Exception("Excel file name has not been provided!");
        FileInfo newFile = new FileInfo(outputDir.FullName + @"\" + ExcelFileName);
        if (newFile.Exists)
        {
            newFile.Delete();  // create a new file everytime
            newFile = new FileInfo(outputDir.FullName + @"\" + ExcelFileName);
        }

        if (this.DatatableToExport == null) throw new Exception("No data to export!");
        if (this.DatatableToExport != null)
        {
            using (ExcelPackage pck = new ExcelPackage())
            {
                //set the workbook properties and add a default sheet in it
                SetWorkbookProperties(pck);

                //Create a sheet
                var ws = pck.Workbook.Worksheets.Add(DatatableToExport.TableName);

                //row index of the excel sheet
                int rowIndex = 1;

                CreateHeader(ws, ref rowIndex, DatatableToExport);
                CreateData(ws, ref rowIndex, DatatableToExport);

                pck.SaveAs(newFile);
            }
        }
        return true;
    }

    /// <summary>
    /// Export Data from datatable to Excel sheet
    /// </summary>
    /// <returns></returns>
    public bool CreateExcelFromObjectList<T>(IList<T> itemList, string worksheetName, bool IsOverride = false, ExcelPackage package = null) where T : class, new()
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

        var columns = typeof(T).GetProperties().Select(pi => pi.Name).ToArray();

        ExcelPackage pck = package;
        if (package == null)
            pck = new ExcelPackage();

        //set the workbook properties and add a default sheet in it
        SetWorkbookProperties(pck);

        //Create a sheet
        var ws = pck.Workbook.Worksheets.Add(worksheetName);

        CreateHeader(ws, columns);
        CreateData<T>(ws, itemList);

        //pck.SaveAs(newFile);

        return true;
    }

    public bool CreateExcelFromObjectList<T>(IList<T> itemList, string worksheetName, string TemplatePath, bool IsOverride = false, ExcelPackage package = null) where T : class, new()
    {
        if (string.IsNullOrEmpty(ExcelFilePath)) throw new Exception("Excel file path has not been provided!");
        DirectoryInfo outputDir = new DirectoryInfo(Path.GetDirectoryName(ExcelFilePath));
        if (!outputDir.Exists) throw new Exception("Excel file path does not exist!");

        if (string.IsNullOrEmpty(ExcelFileName)) throw new Exception("Excel file name has not been provided!");
        FileInfo newFile = new FileInfo(outputDir.FullName + @"\" + ExcelFileName);
        if (TemplatePath != null)
        {
            File.Copy(TemplatePath, newFile.FullName, true);
        }
        if (newFile.Exists)
        {
            if (IsOverride)
                newFile.Delete();  // create a new file everytime
            newFile = new FileInfo(outputDir.FullName + @"\" + ExcelFileName);
            if (TemplatePath != null)
            {
                File.Copy(TemplatePath, newFile.FullName, true);
            }
        }

        //var columns = typeof(T).GetProperties().Select(pi => pi.Name).ToArray();

        ExcelPackage pck = package;
        if (package == null)
            pck = new ExcelPackage();

        //set the workbook properties and add a default sheet in it
        SetWorkbookProperties(pck);

        //Create a sheet
        //var ws = pck.Workbook.Worksheets.Add(worksheetName);

        //CreateHeader(ws, columns);
        if (package.Workbook.Worksheets.Count > 0)
        {
            CreateDataCustomReport<T>(package.Workbook.Worksheets.ElementAt(0), itemList);
        }

        //pck.SaveAs(newFile);

        return true;
    }


    public bool CreateExcelFromObjectListRNDCIQOSS<T>(IList<T> itemList, string worksheetName, string TemplatePath, bool IsOverride = false, ExcelPackage package = null) where T : class, new()
    {
        if (string.IsNullOrEmpty(ExcelFilePath)) throw new Exception("Excel file path has not been provided!");
        DirectoryInfo outputDir = new DirectoryInfo(Path.GetDirectoryName(ExcelFilePath));
        if (!outputDir.Exists) throw new Exception("Excel file path does not exist!");

        if (string.IsNullOrEmpty(ExcelFileName)) throw new Exception("Excel file name has not been provided!");
        FileInfo newFile = new FileInfo(outputDir.FullName + @"\" + ExcelFileName);
        if (TemplatePath != null)
        {
            File.Copy(TemplatePath, newFile.FullName, true);
        }
        if (newFile.Exists)
        {
            if (IsOverride)
                newFile.Delete();  // create a new file everytime
            newFile = new FileInfo(outputDir.FullName + @"\" + ExcelFileName);
            if (TemplatePath != null)
            {
                File.Copy(TemplatePath, newFile.FullName, true);
            }
        }

        //var columns = typeof(T).GetProperties().Select(pi => pi.Name).ToArray();

        ExcelPackage pck = package;
        if (package == null)
            pck = new ExcelPackage();

        //set the workbook properties and add a default sheet in it
        SetWorkbookProperties(pck);

        //Create a sheet
        //var ws = pck.Workbook.Worksheets.Add(worksheetName);

        //CreateHeader(ws, columns);
        if (package.Workbook.Worksheets.Count > 0)
        {
            CreateDataCustomReport<T>(package.Workbook.Worksheets.ElementAt(0), itemList);
        }

        //pck.SaveAs(newFile);

        return true;
    }

    /// <summary>
    /// Create the sheet header row
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="rowIndex"></param>
    /// <param name="dt"></param>
    private static void CreateHeader(ExcelWorksheet ws, string[] Columns)
    {
        int colIndex = 1;
        foreach (string dc in Columns) //Creating Headings
        {
            var cell = ws.Cells[1, colIndex];

            //Setting the background color of header cells to Gray
            var fill = cell.Style.Fill;
            fill.PatternType = ExcelFillStyle.Solid;
            fill.BackgroundColor.SetColor(Color.Gray);

            //Setting Top/left,right/bottom borders.
            var border = cell.Style.Border;
            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

            //Setting Value in cell
            cell.Value = dc;

            colIndex++;
        }
    }

    /// <summary>
    /// Populate the data into the sheet
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="rowIndex"></param>
    /// <param name="dt"></param>
    private static void CreateData<T>(ExcelWorksheet ws, IList<T> dataItems, int startRowIndex = 1) where T : class, new()
    {
        int colIndex = 0;
        int rowIndex = startRowIndex;
        PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (T dr in dataItems) // Adding Data into rows
        {
            colIndex = 1;
            rowIndex++;

            foreach (PropertyInfo dc in properties)
            {
                var cell = ws.Cells[rowIndex, colIndex];

                //Setting Value in cell
                cell.Value = Convert.ToString(dc.GetValue(dr, null));

                //Setting borders of cell
                var border = cell.Style.Border;
                border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
                colIndex++;
            }
        }
    }

    /// <summary>
    /// Populate the data into the sheet
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="rowIndex"></param>
    /// <param name="dt"></param>
    private static void CreateDataCustomReport<T>(ExcelWorksheet ws, IList<T> dataItems, int startRowIndex = 1) where T : class, new()
    {
        int colIndex = 0;
        int rowIndex = startRowIndex;
        //PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        PropertyInfo[] properties = typeof(T).GetProperties();

        foreach (T dr in dataItems) // Adding Data into rows
        {
            colIndex = 1;
            rowIndex++;

            foreach (PropertyInfo dc in properties)
            {
                var cell = ws.Cells[rowIndex, colIndex];

                //Setting Value in cell
                cell.Value = Convert.ToString(dc.GetValue(dr, null));

                //Setting borders of cell
                var border = cell.Style.Border;
                border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
                colIndex++;
            }
        }
    }


    public bool Open()
    {
        bool bflag = true;

        try
        {
            using (ExcelPackage pkg = new ExcelPackage(new FileInfo(this.ExcelFilePath)))
            {
                this.Workbook = pkg.Workbook;
                ExcelWorksheet ws = this.Workbook.Worksheets[1];

                XDocument document = new XDocument();
                XElement header = new XElement("HeaderRow");
                document.Add(header);
                XElement itemElement = null;
                object value = null;
                for (int iIndex = ws.Dimension.Start.Column; iIndex <= ws.Dimension.End.Column; iIndex++)
                {
                    value = ws.Cells[ws.Dimension.Start.Row + 1, iIndex].Value;
                    itemElement = new XElement(ws.Cells[ws.Dimension.Start.Row, iIndex].Value.ToString());
                    itemElement.Add(new XAttribute("DataType", value.GetType()));
                    header.Add(itemElement);
                }
                this.XmlSchema = document;
            }
        }
        catch (Exception) { bflag = false; }

        return bflag;
    }

    #endregion "Public Methods"

    #region ExportWithTemplates

    public MemoryStream CreateExcelFromDataTableWithTemplate(string templateFilePath, DataTable dataTableToExport, string workSheetName)
    {
        FileInfo TemplateFile = new FileInfo(templateFilePath);
        DatatableToExport = dataTableToExport;

        if (!TemplateFile.Exists)
            throw new Exception("Template Excel file is not found");
        if (this.DatatableToExport == null)
            throw new Exception("No data to export!");

        ExcelFileName = Path.GetFileName(TemplateFile.Name);
        ExcelFilePath = templateFilePath;
        Guid NewFileNameGUID = Guid.NewGuid();
        DirectoryInfo outputDir = TemplateFile.Directory;

        string NewFileToGenerate = outputDir.FullName + @"\" + NewFileNameGUID.ToString() + '~' + Path.GetFileName(TemplateFile.Name);
        FileInfo newFile = new FileInfo(NewFileToGenerate);

        if (newFile.Exists)
        {
            newFile.Delete();  // create a new file everytime
            newFile = new FileInfo(NewFileToGenerate);
            File.Copy(TemplateFile.Name, NewFileToGenerate, true);
        }
        else
            File.Copy(TemplateFile.FullName, NewFileToGenerate, true);


        if (this.DatatableToExport != null)
        {
            using (ExcelPackage pck = new ExcelPackage(newFile))
            {
                ExcelWorkbook workBook = pck.Workbook;
                ExcelWorksheet ws = null;
                DataTable dt = DatatableToExport;
                ws = workBook.Worksheets[workSheetName];
                int rowIndex = 1;
                if (ws != null)
                    CreateData(ws, ref rowIndex, dt);
                pck.Save();

            }
        }

        MemoryStream ms = new MemoryStream();
        using (FileStream fileStream = new FileStream(newFile.FullName, FileMode.Open, FileAccess.Read))
        {
            ms.Seek(0, SeekOrigin.Begin);
            fileStream.CopyTo(ms);
        }
        newFile.Delete();
        ms.Position = 0;
        return ms;
    }

    /// <summary>
    /// Creates a Excel work book with given dataset
    /// </summary>
    /// <param name="TemplateFile">Template File</param>
    /// <returns>Memory Stream of Created Sheets</returns>
    /// <remarks >Every datatble in dataset will written as worksheet</remarks>
    public MemoryStream CreateExcelFromDataSetWithTemplate(string templateFilePath, DataSet datasetToExport)
    {
        FileInfo TemplateFile = new FileInfo(templateFilePath);
        DatasetToExport = datasetToExport;

        if (!TemplateFile.Exists)
            throw new Exception("Template Excel file is not found");
        if (this.DatasetToExport == null)
            throw new Exception("No data to export!");

        ExcelFileName = Path.GetFileName(TemplateFile.Name);
        ExcelFilePath = templateFilePath;
        Guid NewFileNameGUID = Guid.NewGuid();
        DirectoryInfo outputDir = TemplateFile.Directory;

        string NewFileToGenerate = outputDir.FullName + @"\" + NewFileNameGUID.ToString() + '~' + Path.GetFileName(TemplateFile.Name);
        FileInfo newFile = new FileInfo(NewFileToGenerate);

        if (newFile.Exists)
        {
            newFile.Delete();  // create a new file everytime
            newFile = new FileInfo(NewFileToGenerate);
            File.Copy(TemplateFile.Name, NewFileToGenerate, true);
        }
        else
            File.Copy(TemplateFile.FullName, NewFileToGenerate, true);


        if (this.DatasetToExport != null)
        {
            using (ExcelPackage pck = new ExcelPackage(newFile))
            {
                ExcelWorkbook workBook = pck.Workbook;
                ExcelWorksheet ws = null;
                for (int i = 0; i < DatasetToExport.Tables.Count; i++)
                {
                    DataTable dt = DatasetToExport.Tables[i];
                    ws = workBook.Worksheets.ElementAt(i); ;
                    if (ws == null)
                        ws = CreateSheet(pck, "Sheet" + (i).ToString());////Create a sheet
                    int rowIndex = 1;
                    if (ws != null)
                        CreateData(ws, ref rowIndex, dt);
                }
                pck.Save();

            }
        }

        MemoryStream ms = new MemoryStream();
        using (FileStream fileStream = new FileStream(newFile.FullName, FileMode.Open, FileAccess.Read))
        {
            ms.Seek(0, SeekOrigin.Begin);
            fileStream.CopyTo(ms);
        }
        newFile.Delete();
        ms.Position = 0;
        return ms;
    }

    public MemoryStream CreateExcelFromDataSetDifference(string templateFilePath, DataSet datasetToExport, string Template)
    {
        FileInfo OutputDifferenceFile = new FileInfo(templateFilePath);
        DatasetToExportDifference = datasetToExport;

        if (!OutputDifferenceFile.Exists)
            throw new Exception("Difference Template Excel file is not found");
        if (this.DatasetToExport == null)
            throw new Exception("No data to export!");

        //ExcelFileName = Path.GetFileName(TemplateFile.Name);
        ExcelFilePath = templateFilePath;
        Guid NewFileNameGUID = Guid.NewGuid();
        DirectoryInfo outputDir = OutputDifferenceFile.Directory;

        string NewFileToGenerate = outputDir.FullName + @"\" + NewFileNameGUID.ToString() + '~' + Path.GetFileName(OutputDifferenceFile.Name);
        FileInfo newFile = new FileInfo(NewFileToGenerate);

        if (newFile.Exists)
        {
            newFile.Delete();  // create a new file everytime
            newFile = new FileInfo(NewFileToGenerate);
            File.Copy(OutputDifferenceFile.Name, NewFileToGenerate, true);
        }
        else
            File.Copy(OutputDifferenceFile.FullName, NewFileToGenerate, true);


        if (this.DatasetToExportDifference != null)
        {
            using (ExcelPackage pck = new ExcelPackage(newFile))
            {
                ExcelWorkbook workBook = pck.Workbook;
                ExcelWorksheet ws = null;
                for (int i = 0; i < DatasetToExport.Tables.Count; i++)
                {
                    DataTable dt = DatasetToExport.Tables[i];
                    ws = workBook.Worksheets.ElementAt(i); ;
                    if (ws == null)
                        ws = CreateSheetDifference(pck, "Sheet" + (i).ToString());////Create a sheet
                    int rowIndex = 1;
                    if (ws != null)
                    {
                        CreateDataDifference(ws, ref rowIndex, dt, Template);
                    }
                }
                pck.Save();

            }
        }

        MemoryStream ms = new MemoryStream();
        using (FileStream fileStream = new FileStream(newFile.FullName, FileMode.Open, FileAccess.Read))
        {
            ms.Seek(0, SeekOrigin.Begin);
            fileStream.CopyTo(ms);
        }
        newFile.Delete();
        ms.Position = 0;
        return ms;
    }

    public MemoryStream CreateExcelFromDataSetOnTheFlyDifference(string templateFilePath, DataSet datasetToExport, string Template, string differencePath, Guid UniqueId)
    {
        MemoryStream ms = new MemoryStream();
        try
        {
            string outputPath = differencePath;
            string fileURL = string.Empty;
            FileInfo OutputDifferenceFile = new FileInfo(templateFilePath);
            DatasetToExportDifference = datasetToExport;

            if (!OutputDifferenceFile.Exists)
                throw new Exception("Difference Template Excel file is not found");
            if (this.DatasetToExport == null)
                throw new Exception("No data to export!");

            //ExcelFileName = Path.GetFileName(TemplateFile.Name);
            ExcelFilePath = templateFilePath;
            //Guid NewFileNameGUID = Guid.NewGuid();
            Guid NewFileNameGUID = UniqueId;
            DirectoryInfo outputDir = OutputDifferenceFile.Directory;

            //string NewFileToGenerate = outputDir.FullName + @"\" + NewFileNameGUID.ToString() + '~' + Path.GetFileName(OutputDifferenceFile.Name);
            string NewFileToGenerate = outputPath + @"\" + NewFileNameGUID.ToString() + '~' + Path.GetFileName(OutputDifferenceFile.Name);

            FileInfo newFile = new FileInfo(NewFileToGenerate);

            if (newFile.Exists)
            {
                newFile.Delete();  // create a new file everytime
                newFile = new FileInfo(NewFileToGenerate);
                File.Copy(OutputDifferenceFile.Name, NewFileToGenerate, true);
            }
            else
                File.Copy(OutputDifferenceFile.FullName, NewFileToGenerate, true);


            if (this.DatasetToExportDifference != null)
            {
                using (ExcelPackage pck = new ExcelPackage(newFile))
                {
                    ExcelWorkbook workBook = pck.Workbook;
                    ExcelWorksheet ws = null;
                    for (int i = 0; i < DatasetToExport.Tables.Count; i++)
                    {
                        DataTable dt = DatasetToExport.Tables[i];
                        ws = workBook.Worksheets.ElementAt(i); ;
                        if (ws == null)
                            ws = CreateSheetDifference(pck, "Sheet" + (i).ToString());////Create a sheet
                        int rowIndex = 1;
                        if (ws != null)
                        {
                            CreateDataDifference(ws, ref rowIndex, dt, Template);
                        }
                    }
                    pck.Save();

                }

            }


            //using (FileStream fileStream = new FileStream(newFile.FullName, FileMode.Open, FileAccess.Read))
            //{
            //    ms.Seek(0, SeekOrigin.Begin);
            //    fileStream.CopyTo(ms);
            //}
            //newFile.Delete();
            //ms.Position = 0;
        }
        catch (Exception ex)
        {
            //Log Exception
        }
        return ms;

        //fileURL = NewFileToGenerate;
        //return fileURL;
    }

    public MemoryStream CreateExcelFromDataSetRFDSDifference(string templateFilePath, DataSet datasetToExport, string Template)
    {
        FileInfo OutputDifferenceFile = new FileInfo(templateFilePath);
        DatasetToExportDifference = datasetToExport;

        if (!OutputDifferenceFile.Exists)
            throw new Exception("Difference Template Excel file is not found");
        if (this.DatasetToExport == null)
            throw new Exception("No data to export!");

        //ExcelFileName = Path.GetFileName(TemplateFile.Name);
        ExcelFilePath = templateFilePath;
        Guid NewFileNameGUID = Guid.NewGuid();
        DirectoryInfo outputDir = OutputDifferenceFile.Directory;

        string NewFileToGenerate = outputDir.FullName + @"\" + NewFileNameGUID.ToString() + '~' + Path.GetFileName(OutputDifferenceFile.Name);
        FileInfo newFile = new FileInfo(NewFileToGenerate);

        if (newFile.Exists)
        {
            newFile.Delete();  // create a new file everytime
            newFile = new FileInfo(NewFileToGenerate);
            File.Copy(OutputDifferenceFile.Name, NewFileToGenerate, true);
        }
        else
            File.Copy(OutputDifferenceFile.FullName, NewFileToGenerate, true);


        if (this.DatasetToExportDifference != null)
        {
            using (ExcelPackage pck = new ExcelPackage(newFile))
            {
                ExcelWorkbook workBook = pck.Workbook;
                ExcelWorksheet ws = null;
                for (int i = 0; i < DatasetToExport.Tables.Count; i++)
                {
                    DataTable dt = DatasetToExport.Tables[i];
                    ws = workBook.Worksheets.ElementAt(i); ;
                    if (ws == null)
                        ws = CreateSheetDifference(pck, "Sheet" + (i).ToString());////Create a sheet
                    int rowIndex = 1;
                    if (ws != null)
                    {
                        CreateDataDifferenceRFDS(ws, ref rowIndex, dt, Template);
                    }
                }
                pck.Save();

            }
        }

        MemoryStream ms = new MemoryStream();
        using (FileStream fileStream = new FileStream(newFile.FullName, FileMode.Open, FileAccess.Read))
        {
            ms.Seek(0, SeekOrigin.Begin);
            fileStream.CopyTo(ms);
        }
        newFile.Delete();
        ms.Position = 0;
        return ms;
    }

    public MemoryStream CreateExcelFromDataSetReport(string templateFilePath, DataSet datasetToExport, string Template)
    {
        FileInfo OutputDifferenceFile = new FileInfo(templateFilePath);
        DatasetToExportDifference = datasetToExport;

        if (!OutputDifferenceFile.Exists)
            throw new Exception("Difference Template Excel file is not found");
        if (this.DatasetToExport == null)
            throw new Exception("No data to export!");

        //ExcelFileName = Path.GetFileName(TemplateFile.Name);
        ExcelFilePath = templateFilePath;
        Guid NewFileNameGUID = Guid.NewGuid();
        DirectoryInfo outputDir = OutputDifferenceFile.Directory;

        string NewFileToGenerate = outputDir.FullName + @"\" + NewFileNameGUID.ToString() + '~' + Path.GetFileName(OutputDifferenceFile.Name);
        FileInfo newFile = new FileInfo(NewFileToGenerate);

        if (newFile.Exists)
        {
            newFile.Delete();  // create a new file everytime
            newFile = new FileInfo(NewFileToGenerate);
            File.Copy(OutputDifferenceFile.Name, NewFileToGenerate, true);
        }
        else
            File.Copy(OutputDifferenceFile.FullName, NewFileToGenerate, true);


        if (this.DatasetToExportDifference != null)
        {
            using (ExcelPackage pck = new ExcelPackage(newFile))
            {
                ExcelWorkbook workBook = pck.Workbook;
                ExcelWorksheet ws = null;
                for (int i = 0; i < DatasetToExport.Tables.Count; i++)
                {
                    DataTable dt = DatasetToExport.Tables[i];
                    ws = workBook.Worksheets.ElementAt(i); ;
                    if (ws == null)
                        ws = CreateSheetDifference(pck, "Sheet" + (i).ToString());////Create a sheet
                    int headerRow = 1;
                    int rowIndex = 1;
                    if (ws != null)
                    {
                        CreateHeaderDifference(ws, ref headerRow, dt);
                        CreateApprovedStatusReport(ws, ref rowIndex, dt, Template);
                    }
                }
                pck.Save();

            }
        }

        MemoryStream ms = new MemoryStream();
        using (FileStream fileStream = new FileStream(newFile.FullName, FileMode.Open, FileAccess.Read))
        {
            ms.Seek(0, SeekOrigin.Begin);
            fileStream.CopyTo(ms);
        }
        newFile.Delete();
        ms.Position = 0;
        return ms;
    }
    #endregion

    #region "Private Methods"

    /// <summary>
    /// Create the Excel sheet
    /// </summary>
    /// <param name="p"></param>
    /// <param name="sheetName"></param>
    /// <returns></returns>
    private static ExcelWorksheet CreateSheet(ExcelPackage p, string sheetName)
    {
        p.Workbook.Worksheets.Add(sheetName);
        ExcelWorksheet ws = p.Workbook.Worksheets[1];
        ws.Name = sheetName; //Setting Sheet's name
        ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
        ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

        return ws;
    }


    /// <summary>
    /// Create the sheet header row
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="rowIndex"></param>
    /// <param name="dt"></param>
    private static void CreateHeader(ExcelWorksheet ws, ref int rowIndex, DataTable dt)
    {
        int colIndex = 1;
        int rowcount = dt.Rows.Count;
        foreach (DataColumn dc in dt.Columns) //Creating Headings
        {
            var cell = ws.Cells[rowIndex, colIndex];

            //Setting the background color of header cells to Gray
            var fill = cell.Style.Fill;
            fill.PatternType = ExcelFillStyle.Solid;
            //fill.BackgroundColor.SetColor(Color.Gray);
            fill.BackgroundColor.SetColor(Color.LightBlue);

            //Setting Top/left,right/bottom borders.
            var border = cell.Style.Border;
            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

            //Setting date type format for date columns
            if(dc.DataType == typeof(System.DateTime))
            {
                ws.Cells[2,colIndex,rowcount+1,colIndex].Style.Numberformat.Format = "mm-dd-yy";//or m/d/yy h:mm
            }
            //Setting Value in cell
            cell.Value = dc.ColumnName;

            colIndex++;
        }
    }


    /// <summary>
    /// Populate the data into the sheet
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="rowIndex"></param>
    /// <param name="dt"></param>
    private static void CreateData(ExcelWorksheet ws, ref int rowIndex, DataTable dt)
    {
        int colIndex = 0;
        foreach (DataRow dr in dt.Rows) // Adding Data into rows
        {
            colIndex = 1;
            rowIndex++;

            foreach (DataColumn dc in dt.Columns)
            {
                var cell = ws.Cells[rowIndex, colIndex];
                string typeString = dc.DataType.ToString();
                if (typeString == "System.Int32" || typeString == "System.Int64" || typeString == "System.Double" || typeString == "System.DateTime")
                {
                    cell.Value = dr[dc.ColumnName];
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }
                else
                {
                    //Setting Value in cell
                    cell.Value = Convert.ToString(dr[dc.ColumnName]);
                }
                colIndex++;
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

    #endregion "Private Methods"

    public Export2ExcelLib()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public Export2ExcelLib(string FileName)
    {
        this.ExcelFileName = System.IO.Path.GetFileName(FileName);
        this.ExcelFilePath = FileName;
    }

    #region Design difference check Methods

    /// <summary>
    /// Create the sheet header row
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="rowIndex"></param>
    /// <param name="dt"></param>
    private static void CreateHeaderDifference(ExcelWorksheet ws, ref int rowIndex, DataTable dt)
    {
        int colIndex = 1;
        foreach (DataColumn dc in dt.Columns) //Creating Headings
        {
            var cell = ws.Cells[rowIndex, colIndex];

            //Setting the background color of header cells to Gray
            var fill = cell.Style.Fill;
            fill.PatternType = ExcelFillStyle.Solid;
            fill.BackgroundColor.SetColor(Color.Gray);

            //Setting Top/left,right/bottom borders.
            var border = cell.Style.Border;
            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

            //Setting Value in cell
            cell.Value = dc.ColumnName;

            colIndex++;
        }
    }


    /// <summary>
    /// Populate the data into the sheet
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="rowIndex"></param>
    /// <param name="dt"></param>
    //private static void CreateDataDifference(ExcelWorksheet ws, ref int rowIndex, DataTable dt)
    //{

    //    //var results = from myRow in dt.AsEnumerable()
    //    //              //where myRow.Field<string>("Name") == "ECL00011"
    //    //              select myRow;

    //    int rowCell = 0;

    //    for (int row = 0; row < dt.Rows.Count - 1; row = row + 2)
    //    {


    //        for (int col = 0; col < dt.Columns.Count - 1; col++)
    //        {
    //            if (dt.Rows[row][0].Equals(dt.Rows[row + 1][0]))
    //            {

    //                if (dt.Rows[row][col].Equals(dt.Rows[row + 1][col]))
    //                //if (dt.Rows[row][col].GetHashCode()==dt.Rows[row + 1][col].GetHashCode())
    //                {
    //                    var cell = ws.Cells[row + 2, col + 1];
    //                    //cell.Style.Fill.PatternType = ExcelFillStyle.None;
    //                    //cell.Style.Fill.BackgroundColor.SetColor(Color.White);
    //                    string typeString = dt.Columns[col].DataType.ToString();// dc.DataType.ToString();
    //                    if (typeString == "System.Int32" || typeString == "System.Int64" || typeString == "System.Double")
    //                    {
    //                        cell.Value = dt.Rows[row][col];// dr[dc.ColumnName];
    //                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
    //                        //cell.Style.Fill.BackgroundColor = new ExcelColor("Red")
    //                    }
    //                    else
    //                    {
    //                        //Setting Value in cell
    //                        cell.Value = Convert.ToString(dt.Rows[row][col]);//dr[dc.ColumnName]);
    //                    }

    //                    var cell2 = ws.Cells[row + 3, col + 1];
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
    //                else
    //                {

    //                    var cell = ws.Cells[row + 2, col + 1];
    //                    //cell.Style.Fill.PatternType = ExcelFillStyle.Solid; 
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
    //                    var cell2 = ws.Cells[row + 3, col + 1];
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
    //            }
    //        }

    //    }
    //}

    private static void CreateDataDifference(ExcelWorksheet ws, ref int rowIndex, DataTable dt)
    {

        //var results = from myRow in dt.AsEnumerable()
        //              //where myRow.Field<string>("Name") == "ECL00011"
        //              select myRow;

        int rowCell = 0;

        for (int row = 0; row < dt.Rows.Count - 1; row = row + 2)
        {

            for (int col = 0; col < dt.Columns.Count - 1; col++)
            {
                if (dt.Rows[row][0].Equals(dt.Rows[row + 1][0]))
                {

                    if (dt.Rows[row][col].Equals(dt.Rows[row + 1][col]))
                    //if (dt.Rows[row][col].GetHashCode()==dt.Rows[row + 1][col].GetHashCode())
                    {
                        var cell = ws.Cells[rowCell + 2, col + 1];
                        //cell.Style.Fill.PatternType = ExcelFillStyle.None;
                        //cell.Style.Fill.BackgroundColor.SetColor(Color.White);
                        string typeString = dt.Columns[col].DataType.ToString();// dc.DataType.ToString();
                        if (typeString == "System.Int32" || typeString == "System.Int64" || typeString == "System.Double")
                        {
                            cell.Value = dt.Rows[row][col];// dr[dc.ColumnName];
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            //cell.Style.Fill.BackgroundColor = new ExcelColor("Red")
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
                    else
                    {

                        var cell = ws.Cells[rowCell + 2, col + 1];
                        //cell.Style.Fill.PatternType = ExcelFillStyle.Solid; 
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
                }
            }

            if (dt.Rows[row][0].Equals(dt.Rows[row + 1][0])) rowCell += 2;


        }
    }


    private static void CreateDataDifference(ExcelWorksheet ws, ref int rowIndex, DataTable dt, string template)
    {

        //var results = from myRow in dt.AsEnumerable()
        //              //where myRow.Field<string>("Name") == "ECL00011"
        //              select myRow;
        if (template.ToUpper() == "DIFFERENCE")
        {
            int rowCell = 0;
            if (dt != null && dt.Rows.Count > 1)
            {
                for (int row = 0; row < dt.Rows.Count - 1; row = row + 2)
                {

                    for (int col = 0; col < dt.Columns.Count - 1; col++)
                    {
                        //if (dt.Rows[row][0].Equals(dt.Rows[row + 1][0]))
                        //{

                        if (dt.Rows[row][col].Equals(dt.Rows[row + 1][col]))
                        //if (dt.Rows[row][col].GetHashCode()==dt.Rows[row + 1][col].GetHashCode())
                        {
                            var cell = ws.Cells[rowCell + 2, col + 1];
                            //cell.Style.Fill.PatternType = ExcelFillStyle.None;
                            //cell.Style.Fill.BackgroundColor.SetColor(Color.White);
                            string typeString = dt.Columns[col].DataType.ToString();// dc.DataType.ToString();
                            if (typeString == "System.Int32" || typeString == "System.Int64" || typeString == "System.Double")
                            {
                                cell.Value = dt.Rows[row][col];// dr[dc.ColumnName];
                                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                //cell.Style.Fill.BackgroundColor = new ExcelColor("Red")
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
                        else
                        {

                            var cell = ws.Cells[rowCell + 2, col + 1];
                            //cell.Style.Fill.PatternType = ExcelFillStyle.Solid; 
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

                    //if (dt.Rows[row][0].Equals(dt.Rows[row + 1][0])) 
                    rowCell += 2;


                }
            }

        }
        else //if (template.ToUpper()=="TEMPLATE_DIFFERENCE_RNDCIQ_SUPPLEMENT_DATA_LTE_V2.5.XLSX")
        {
            int rowCell = 0;
            if (dt != null && dt.Rows.Count > 1)
            {
                for (int row = 0; row < dt.Rows.Count - 1; row = row + 2)
                {

                    for (int col = 0; col < dt.Columns.Count - 1; col++)
                    {
                        //if ((Convert.ToString(dt.Rows[row][0]) + Convert.ToString(dt.Rows[row][1]))==(Convert.ToString(dt.Rows[row + 1][0])+Convert.ToString(dt.Rows[row + 1][1])))
                        //{

                        if (dt.Rows[row][col].Equals(dt.Rows[row + 1][col]))
                        //if (dt.Rows[row][col].GetHashCode()==dt.Rows[row + 1][col].GetHashCode())
                        {
                            var cell = ws.Cells[rowCell + 2, col + 1];
                            //cell.Style.Fill.PatternType = ExcelFillStyle.None;
                            //cell.Style.Fill.BackgroundColor.SetColor(Color.White);
                            string typeString = dt.Columns[col].DataType.ToString();// dc.DataType.ToString();
                            if (typeString == "System.Int32" || typeString == "System.Int64" || typeString == "System.Double")
                            {
                                cell.Value = dt.Rows[row][col];// dr[dc.ColumnName];
                                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                //cell.Style.Fill.BackgroundColor = new ExcelColor("Red")
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
                        else
                        {

                            var cell = ws.Cells[rowCell + 2, col + 1];
                            //cell.Style.Fill.PatternType = ExcelFillStyle.Solid; 
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

                    //if (dt.Rows[row][0].Equals(dt.Rows[row + 1][0])) 
                    rowCell += 2;


                }
            }
        }
    }

    private static void CreateDataDifferenceRFDS(ExcelWorksheet ws, ref int rowIndex, DataTable dt, string template)
    {

        //var results = from myRow in dt.AsEnumerable()
        //              //where myRow.Field<string>("Name") == "ECL00011"
        //              select myRow;
        //if (template.ToUpper() == "DIFFERENCE")
        //{
        int rowCell = 0;
        if (dt != null && dt.Rows.Count > 1)
        {
            for (int row = 0; row < dt.Rows.Count - 1; row = row + 2)
            {

                for (int col = 0; col < dt.Columns.Count - 1; col++)
                {
                    //if (dt.Rows[row][0].Equals(dt.Rows[row + 1][0]))
                    //{

                    if (dt.Rows[row][col].Equals(dt.Rows[row + 1][col]))
                    //if (dt.Rows[row][col].GetHashCode()==dt.Rows[row + 1][col].GetHashCode())
                    {
                        var cell = ws.Cells[rowCell + 2, col + 1];
                        //cell.Style.Fill.PatternType = ExcelFillStyle.None;
                        //cell.Style.Fill.BackgroundColor.SetColor(Color.White);
                        string typeString = dt.Columns[col].DataType.ToString();// dc.DataType.ToString();
                        if (typeString == "System.Int32" || typeString == "System.Int64" || typeString == "System.Double")
                        {
                            cell.Value = dt.Rows[row][col];// dr[dc.ColumnName];
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            //cell.Style.Fill.BackgroundColor = new ExcelColor("Red")
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
                    else
                    {

                        var cell = ws.Cells[rowCell + 2, col + 1];
                        //cell.Style.Fill.PatternType = ExcelFillStyle.Solid; 
                        //cell.Style.Font.Color.SetColor(Color.Red);
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

                //if (dt.Rows[row][0].Equals(dt.Rows[row + 1][0])) 
                rowCell += 2;


            }
        }

        //        }


    }

    private static void CreateApprovedStatusReport(ExcelWorksheet ws, ref int rowIndex, DataTable dt, string template)
    {

        //var results = from myRow in dt.AsEnumerable()
        //              //where myRow.Field<string>("Name") == "ECL00011"
        //              select myRow;
        //if (template.ToUpper() == "DIFFERENCE")
        //{

        if (dt != null && dt.Rows.Count > 0)
        {
            for (int row = 0; row < dt.Rows.Count; row++)
            {

                for (int col = 0; col < dt.Columns.Count; col++)
                {

                    var cell = ws.Cells[row + 2, col + 1];
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

                    //var cell2 = ws.Cells[rowCell + 3, col + 1];
                    //cell2.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //cell2.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    //string typeString2 = dt.Columns[col].DataType.ToString();// dc.DataType.ToString();
                    //if (typeString == "System.Int32" || typeString == "System.Int64" || typeString == "System.Double")
                    //{
                    //    cell2.Value = dt.Rows[row + 1][col];// dr[dc.ColumnName];
                    //    cell2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    //}
                    //else
                    //{
                    //    //Setting Value in cell
                    //    cell2.Value = Convert.ToString(dt.Rows[row + 1][col]);//dr[dc.ColumnName]);
                    //}

                }




            }
        }

        //        }


    }
    /// <summary>
    /// Set the properties of the Excel file
    /// </summary>
    /// <param name="p"></param>
    private void SetWorkbookPropertiesDifference(ExcelPackage p)
    {
        //Here setting some document properties
        p.Workbook.Properties.Author = Author;
        p.Workbook.Properties.Title = Title;
    }


    /// <summary>
    /// Create the Excel sheet
    /// </summary>
    /// <param name="p"></param>
    /// <param name="sheetName"></param>
    /// <returns></returns>
    private static ExcelWorksheet CreateSheetDifference(ExcelPackage p, string sheetName)
    {
        p.Workbook.Worksheets.Add(sheetName);
        ExcelWorksheet ws = p.Workbook.Worksheets[1];
        ws.Name = sheetName; //Setting Sheet's name
        ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
        ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

        return ws;
    }

    #endregion Design difference check Methods

}