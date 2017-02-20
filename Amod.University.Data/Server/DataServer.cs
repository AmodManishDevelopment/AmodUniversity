using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Amod.University.Data.Server
{
    public class DataServer : IDataServer
    {
        private static readonly string LEGACY_COMMAND_ENDTEXT = ConfigurationManager.AppSettings["LEGACY_COMMAND_ENDTEXT"] ??
                                                                      @"(.*?)\b[^<](.*?)((></.*?)|(/))(>')"; //^.*\/>'$

        public DataServer()
        {
        }

        #region IDataServer Members
        public List<DataTable> GetData(String storedProcedureName, Dictionary<string, object> parameters = null, String sqlServerAliasName = null, Dictionary<string, DataTable> tableParams = null)
        {
            List<DataTable> resultDataTablesList = new List<DataTable>(1);

            DataSet dataSet = GetDataByDataSet(storedProcedureName, parameters, sqlServerAliasName, tableParams);
            resultDataTablesList.AddRange(dataSet.Tables.Cast<DataTable>());
            return resultDataTablesList;
        }

        /// <summary>
        /// Gets the data via DataReader.
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="sqlServerAliasName"></param>
        /// <param name="tableParams"></param>
        /// <returns></returns>
        public List<DataTable> GetDataByDataReader(string storedProcedureName, Dictionary<string, object> parameters = null, string sqlServerAliasName = null, Dictionary<string, DataTable> tableParams = null)
        {
            var resultDataTablesList = new List<DataTable>(1);
            var connString = "";
            var sb = new StringBuilder();
            try
            {
                    using (var conn = new SqlConnection())
                    {
                        connString = GetConnectionString(sqlServerAliasName);
                        conn.ConnectionString = connString;
                        var comm = new SqlCommand(storedProcedureName, conn) { CommandTimeout = 300 };

                        if (null == parameters && null == tableParams && ContainsLegacyXML(storedProcedureName))
                            comm.CommandText = storedProcedureName;
                        else
                        {
                            comm.CommandType = CommandType.StoredProcedure;
                        }
                        if (null != parameters)
                        {
                            foreach (var paramName in parameters.Keys)
                            {
                                var paramValue = parameters[paramName] ?? DBNull.Value;
                                comm.Parameters.Add(new SqlParameter(paramName, paramValue));
                            }
                        }

                        if (null != tableParams)
                        {
                            foreach (var paramName in tableParams.Keys)
                            {
                                var sqlParam = new SqlParameter(paramName, SqlDbType.Structured)
                                {
                                    Value = tableParams[paramName]
                                };
                                comm.Parameters.Add(sqlParam);
                            }
                        }

                        var dt = new DataTable("OutputTable");
                        conn.Open();
                        using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (reader.Read())
                            {
                                sb.Append(reader.GetValue(0).ToString());
                            }
                            while (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    sb.Append(reader.GetValue(0).ToString());
                                }
                            }
                        }

                        dt.Columns.Add(new DataColumn("OutputXml", typeof(string)));
                        var datarow = dt.NewRow();
                        datarow["OutputXml"] = sb.ToString();
                        dt.Rows.Add(datarow);

                        resultDataTablesList.Add(dt);
                    }
            }
            catch (Exception exp)
            {
                var errDataTable = new DataTable("PROC_CALL_ERROR");
                errDataTable.Columns.Add(new DataColumn("ErrorMessage", typeof(String)));
                var errorRow = errDataTable.NewRow();
                errorRow[0] = exp.Message;
                errDataTable.Rows.Add(errorRow);
                resultDataTablesList.Add(errDataTable);

                //TODO: Logging Result String.Format("Exception {0} while calling stored procedure {1} using connection string {2}.", exp.Message, storedProcedureName, connString), storedProcedureName, exp.ToString(), exp.GetType().Name));
            }
            return (resultDataTablesList);
        }

        public DataTable SetData(string storedProcedureName, Dictionary<String, object> parameters = null, String sqlServerAliasName = null, Dictionary<string, DataTable> tableParams = null)
        {
            Int32 rowsAffected = -1;
            String connString = "";
            DataTable result = null;
            try
            {
                    using (SqlConnection conn = new SqlConnection())
                    {
                        connString = GetConnectionString(sqlServerAliasName);
                        conn.ConnectionString = connString;
                        SqlCommand comm = new SqlCommand(storedProcedureName, conn);

                        if (null == parameters && null == tableParams && ContainsLegacyXML(storedProcedureName))
                            comm.CommandText = storedProcedureName;
                        else
                        {
                            comm.CommandType = CommandType.StoredProcedure;
                        }

                        if (null != parameters)
                        {
                            foreach (String paramName in parameters.Keys)
                            {
                                Object paramValue = parameters[paramName];
                                if (null == paramValue)
                                    paramValue = DBNull.Value;
                                comm.Parameters.Add(new SqlParameter(paramName, paramValue));
                            }
                        }

                        if (null != tableParams)
                        {
                            foreach (string paramName in tableParams.Keys)
                            {
                                var sqlParam = new SqlParameter(paramName, SqlDbType.Structured)
                                {
                                    Value = tableParams[paramName]
                                };
                                comm.Parameters.Add(sqlParam);
                            }
                        }
                        conn.Open();
                        rowsAffected = comm.ExecuteNonQuery();
                        conn.Close();
                        DataTable resultDT = new DataTable("Result");
                        resultDT.Columns.Add(new DataColumn("RowsAffected", typeof(Int32)));
                        DataRow dr = resultDT.NewRow();
                        dr[0] = rowsAffected;
                        resultDT.Rows.Add(dr);
                        result = resultDT;
                    }
            }
            catch (Exception exp)
            {
                DataTable errDataTable = new DataTable("PROC_CALL_ERROR");
                errDataTable.Columns.Add(new DataColumn("ErrorMessage", typeof(String)));
                DataRow errorRow = errDataTable.NewRow();
                errorRow[0] = exp.Message;
                errDataTable.Rows.Add(errorRow);
                result = errDataTable;

                //TODO: Logging Result String.Format("Exception {0} while calling stored procedure {1} using connection string {2}.", exp.Message, storedProcedureName, connString), storedProcedureName, exp.ToString(), exp.GetType().Name)));
            }

            return (result);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns if the passed parameter has any xml in it
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <returns></returns>
        private bool ContainsLegacyXML(string storedProcedureName)
        {
            Regex expression = new Regex(@LEGACY_COMMAND_ENDTEXT);
            return expression.IsMatch(storedProcedureName);
        }

        private String GetConnectionString(String sqlServerAliasName)
        {
            String connectionString = null;
            try
            {
                connectionString = ConfigurationManager.AppSettings["SQLConnStr"].ToString(); 
            }
            catch (Exception exp)
            {
                //TODO: Logging exp
            }

            return (connectionString);
        }

        private DataSet GetDataByDataSet(string storedProcedureName, Dictionary<string, object> parameters, string sqlServerAliasName, Dictionary<string, DataTable> tableParams)
        {
            DataSet dataSet = new DataSet();
            String connString = "";
            try
            {
                    using (var conn = new SqlConnection())
                    {
                        connString = GetConnectionString(sqlServerAliasName);
                        conn.ConnectionString = connString;
                        var comm = new SqlCommand(storedProcedureName, conn) { CommandTimeout = 300 };

                        if (null == parameters && null == tableParams && ContainsLegacyXML(storedProcedureName))
                            comm.CommandText = storedProcedureName;
                        else
                        {
                            comm.CommandType = CommandType.StoredProcedure;
                        }
                        if (null != parameters)
                        {
                            foreach (String paramName in parameters.Keys)
                            {
                                object paramValue = parameters[paramName] ?? DBNull.Value;
                                comm.Parameters.Add(new SqlParameter(paramName, paramValue));
                            }
                        }

                        if (null != tableParams)
                        {
                            foreach (string paramName in tableParams.Keys)
                            {
                                var sqlParam = new SqlParameter(paramName, SqlDbType.Structured)
                                {
                                    Value = tableParams[paramName]
                                };
                                comm.Parameters.Add(sqlParam);
                            }
                        }

                        var adapter = new SqlDataAdapter(comm);

                        dataSet = new DataSet();
                        adapter.Fill(dataSet);
                    }
            }
            catch (Exception exp)
            {
                DataTable errDataTable = new DataTable("PROC_CALL_ERROR");
                errDataTable.Columns.Add(new DataColumn("ErrorMessage", typeof(String)));
                DataRow errorRow = errDataTable.NewRow();
                errorRow[0] = exp.Message;
                errDataTable.Rows.Add(errorRow);
                dataSet.Tables.Add(errDataTable);

                //TODO: Logging Result String.Format("Exception {0} while calling stored procedure {1} using connection string {2}.", exp.Message, storedProcedureName, connString), storedProcedureName, exp.ToString(), exp.GetType().Name));
            }
            return (dataSet);
        }

        #endregion
    }
}
