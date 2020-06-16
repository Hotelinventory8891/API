using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IStoredProcedure<TResult>
    {

    }

    public interface IStoredProcedure
    {

    }

    public static class DatabaseExtensions
    {
        public static IEnumerable<TResult> ExecuteStoredProcedure<TResult>(this Database database, IStoredProcedure<TResult> procedure)
        {
            var parameters = CreateSqlParametersFromProperties(procedure);
            var format = CreateSPCommand<TResult>(parameters);
            return database.SqlQuery<TResult>(format, parameters.Cast<object>().ToArray());
        }


        private static List<SqlParameter> CreateSqlParametersFromProperties<TResult>(IStoredProcedure<TResult> procedure)
        {
            var procedureType = procedure.GetType();
            var propertiesOfProcedure = procedureType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var parameters =
                propertiesOfProcedure.Select(propertyInfo => new SqlParameter(string.Format("@{0}", (object)propertyInfo.Name),
                                                                              propertyInfo.GetValue(procedure, new object[] { })))
                    .ToList();
            return parameters;
        }

        private static string CreateSPCommand<TResult>(List<SqlParameter> parameters)
        {
            var name = typeof(TResult).Name;
            string queryString = string.Format("usp_{0}", name);
            parameters.ForEach(x => queryString = string.Format("{0} {1},", queryString, x.ParameterName));
            return queryString.TrimEnd(',');
        }

        private static List<SqlParameter> CreateSqlParametersFromProperties(IStoredProcedure procedure)
        {
            var procedureType = procedure.GetType();
            var propertiesOfProcedure = procedureType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var parameters =
                propertiesOfProcedure.Select(propertyInfo => new SqlParameter(string.Format("@{0}", (object)propertyInfo.Name),
                                                                              propertyInfo.GetValue(procedure, new object[] { })))
                    .ToList();
            return parameters;
        }

        private static string CreateSPCommand(string name, List<SqlParameter> parameters)
        {
            string queryString = string.Format("usp_{0}", name);
            parameters.ForEach(x => queryString = string.Format("{0} {1},", queryString, x.ParameterName));
            return queryString.TrimEnd(',');
        }

        public static DataSet ExecuteStoredProcedure(this Database database, IStoredProcedure procedure)
        {
            var ds = new DataSet();
            var name = string.Format("usp_{0}", procedure.GetType().Name);
            var procedureType = procedure.GetType();
            var propertiesOfProcedure = procedureType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            SqlCommand cmd = new SqlCommand(name);
            foreach (var property in propertiesOfProcedure)
                cmd.Parameters.AddWithValue(string.Format("@{0}", property.Name), property.GetValue(procedure, new object[] { }));
            using (var conn = new SqlConnection(database.Connection.ConnectionString))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(ds);
                }
            }
            return ds;
        }


    }
}
