using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ConverttoDataTable
    {
        public static DataTable ToDataTable<T>(List<T> items)
        {
            try
            {
                DataTable dataTable = new System.Data.DataTable(typeof(T).Name);

                //Get all the properties
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    Type datatype = prop.PropertyType;                    
                    DataColumn dc = new DataColumn();
                    if (prop.PropertyType.FullName.IndexOf("Nullable`1") > 0)
                    {                        
                        switch(((prop.PropertyType.FullName.Split('['))[2].Split(','))[0])
                        {
                            case "System.Int32":
                                datatype = typeof(Int32);
                                break;
                            case "System.Int64":
                                datatype = typeof(Int64);
                                break;
                            case "System.Double":
                                datatype = typeof(Double);
                                break;
                            case "System.DateTime":
                                datatype = typeof(DateTime);
                                break;
                            case "System.Boolean":
                                datatype = typeof(Boolean);
                                break;
                            default:
                                break;
                        }
                    }
                    //Setting column names as Property display name attribute or property names
                    if (((DisplayAttribute)((System.Attribute[])(prop.GetCustomAttributes()))[0]).Name != null)
                    {
                        dc = new DataColumn()
                        {
                            ColumnName = ((DisplayAttribute)((System.Attribute[])(prop.GetCustomAttributes()))[0]).Name,
                            DataType = datatype,
                            AllowDBNull = true
                        };
                    }
                    else
                    {
                        dc = new DataColumn()
                        {
                            ColumnName = dataTable.Columns.Add(prop.Name).ToString(),
                            DataType = datatype,
                            AllowDBNull = true
                        };
                    }

                    dataTable.Columns.Add(dc);
                }
                foreach (T item in items)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                        values[i] = Props[i].GetValue(item, null);
                        //string typeString = Props[i].PropertyType.ToString();
                        //switch (typeString)
                        //{
                            
                        //    case "System.Int32":
                        //        values[i] = Convert.ToInt32(Props[i].GetValue(item, null));
                        //        break;
                        //    case "System.Int64":
                        //        values[i] = Convert.ToInt64(Props[i].GetValue(item, null));
                        //        break;
                        //    case "System.Double":
                        //        values[i] = Convert.ToDouble(Props[i].GetValue(item, null));
                        //        break;
                        //    default:
                        //        values[i] = Convert.ToString(Props[i].GetValue(item, null));
                        //        break;
                        //}
                        //if (typeString == "System.Int32" || typeString == "System.Int64" || typeString == "System.Double")
                        //{
                        //    values[i] = Props[i].GetValue(item, null);                            
                        //}
                        //else
                        //{
                        //    //Setting Value in cell
                        //    values[i] = Convert.ToString(values[i] = Props[i].GetValue(item, null));
                        //}
                        //inserting property values to datatable rows
                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }
                //put a breakpoint here and check datatable
                return dataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
