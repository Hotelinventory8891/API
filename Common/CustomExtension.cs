using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Common
{
    public static class CustomExtension
    {
        public static AutoMapper.IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(this AutoMapper.IMappingExpression<TSource, TDestination> expression)
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);
            var existingMaps = AutoMapper.Mapper.GetAllTypeMaps().First(x => x.SourceType.Equals(sourceType)
                && x.DestinationType.Equals(destinationType));
            foreach (var property in existingMaps.GetUnmappedPropertyNames())
            {
                expression.ForMember(property, opt => opt.Ignore());
            }
            return expression;
        }

        public static string genreateExcel<T>(this IEnumerable<T> ExportList, string sheetname) where T : class, new()
        {
      
            return ExportList.ToList().genreateExcel(sheetname);
          
        }

        public static string genreateExcel<T>(this IList<T> ExportList, string sheetname) where T : class, new()
        {
            Guid guid = Guid.NewGuid();
            string foldername = System.Configuration.ConfigurationManager.AppSettings["TempPath"];
            if (!Directory.Exists(foldername))
                Directory.CreateDirectory(foldername);
            string filename = string.Format(@"{0}\{1}.xlsx", foldername, guid);
            Export2ExcelLib export2Excel = new Export2ExcelLib(filename);
            export2Excel.Title = "File Export";
            FileInfo file = new FileInfo(filename);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                export2Excel.CreateExcelFromObjectList<T>(ExportList, sheetname, package: package);
                package.Save();
            }
            return filename;
        }
    }
}
