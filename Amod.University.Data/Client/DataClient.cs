using Amod.University.Data.Helper;
using Amod.University.Data.Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Amod.University.Data.Client
{
    /// <summary>
    /// Data Client class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [KnownType(typeof(SqlParameter))]
    [KnownType(typeof(SqlInt32))]
    public class DataClient<T> : IDataClient<T>
    {
        private static Dictionary<Type, PropertyInfo[]> _PropertiesDictionaryCache = new Dictionary<Type, PropertyInfo[]>(1000);

        /// <summary>
        /// C'tor for a named url
        /// </summary>
        /// <param name="url"></param>
        public DataClient()
        {
        }

        #region IDataClient<T> Members

        /// <summary>
        /// Call a stored procedure and return a list of typed objects
        /// This method works when stored procedure is returning multiple resultsets.
        /// For procs returning single resultset and get typed objects result, use
        /// GetTypedData instead.
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="typesOfObjectsToReturn">The types of objects to return.</param>
        /// <param name="typeResultObjectsDictionary">The type result objects dictionary.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="sqlServerAliasName">Name of the SQL server alias.</param>
        /// <param name="tableParams">Table Valued Parameters that don't get serialized as part of objects</param>
        /// <returns>
        /// ServiceResult
        /// </returns>
        public DataClientResult GetMultiTypedData(String storedProcedureName,
            List<Type> typesOfObjectsToReturn,
            out Dictionary<Type, List<Object>> typeResultObjectsDictionary,
            Dictionary<String, Object> parameters = null,
            String sqlServerAliasName = null, Dictionary<string, DataTable> tableParams = null)
        {
            DataClientResult result = new DataClientResult();
            typeResultObjectsDictionary = new Dictionary<Type, List<Object>>(10);

            try
            {
                List<DataTable> dataTablesList = null;
                result = CallDataServer(storedProcedureName, parameters, sqlServerAliasName, out dataTablesList, false, tableParams);

                if (result.IsSuccess)
                {
                    for (Int32 index = 0; index < typesOfObjectsToReturn.Count; index++)
                    {
                        Type objectType = typesOfObjectsToReturn[index];
                        List<Object> typedDataList = new List<Object>(1000);
                        PropertyInfo[] typePropertiesArr = null;

                        if (null != objectType)
                        {
                            if (_PropertiesDictionaryCache.ContainsKey(objectType))
                            {
                                typePropertiesArr = _PropertiesDictionaryCache[objectType];
                            }
                            else
                            {
                                typePropertiesArr = objectType.GetProperties();
                                _PropertiesDictionaryCache.Add(objectType, typePropertiesArr);
                            }
                        }

                        if (null != objectType && null != typePropertiesArr)
                        {
                            foreach (DataRow dr in dataTablesList[index].Rows)
                            {
                                var returnObj = Activator.CreateInstance(objectType);

                                foreach (PropertyInfo prop in typePropertiesArr)
                                {
                                    if (prop.CanWrite)
                                    {
                                        Type nullableUnderlyingType = Nullable.GetUnderlyingType(prop.PropertyType);
                                        if (dr.Table.Columns.Contains(prop.Name) && DBNull.Value != dr[prop.Name])
                                        {
                                            var typedValue = (null != nullableUnderlyingType) ?
                                                   Convert.ChangeType(dr[prop.Name], nullableUnderlyingType) :
                                                   Convert.ChangeType(dr[prop.Name], prop.PropertyType);
                                            prop.SetValue(returnObj, typedValue, null);
                                        }
                                    }
                                }

                                typedDataList.Add(returnObj);
                            }

                            typeResultObjectsDictionary.Add(objectType, typedDataList);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.ErrorException = exp;
                result.Message = String.Format("Exception {0} in DataClient::GetMultiTypedData method. See exception stack trace for more details.", exp.Message);
                result.IsOperationalError = true;
            }

            if (false == result.IsSuccess)
            {
                //TODO: Logging Result
            }
            return (result);
        }


        /// <summary>
        /// Call a stored procedure and return a list of typed objects
        /// This method works when stored procedure is only returning one resultset.
        /// For multiple resultset returning procs for typed objects result, use 
        /// GetMultiTypedData
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="typedDataList">The typed data list.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="sqlServerAliasName">Name of the SQL server alias.</param>
        /// <param name="tableParams">Table Valued Parameters that don't get serialized as part of objects</param>

        /// <returns>ServiceResult</returns>
        public DataClientResult GetTypedData(String storedProcedureName,
                                            out List<T> typedDataList,
                                            Dictionary<String, Object> parameters = null,
                                            String sqlServerAliasName = null, Dictionary<string, DataTable> tableParams = null)
        {
            DataClientResult result = new DataClientResult();
            typedDataList = new List<T>(1000);

            try
            {
                List<DataTable> dataTablesList = null;
                result = CallDataServer(storedProcedureName, parameters, sqlServerAliasName, out dataTablesList, false, tableParams);

                if (result.IsSuccess)
                {
                    Type objectType = null;
                    PropertyInfo[] typePropertiesArr = null;

                    objectType = typeof(T);
                    if (null != objectType)
                    {
                        if (_PropertiesDictionaryCache.ContainsKey(objectType))
                        {
                            typePropertiesArr = _PropertiesDictionaryCache[objectType];
                        }
                        else
                        {
                            typePropertiesArr = objectType.GetProperties();
                            _PropertiesDictionaryCache.Add(objectType, typePropertiesArr);
                        }
                    }

                    if (null != objectType && null != typePropertiesArr)
                    {
                        foreach (DataRow dr in dataTablesList[0].Rows)
                        {
                            T returnObj = (T)Activator.CreateInstance(typeof(T));

                            foreach (PropertyInfo prop in typePropertiesArr)
                            {
                                if (prop.CanWrite)
                                {
                                    if (dr.Table.Columns.Contains(prop.Name) && DBNull.Value != dr[prop.Name] && dr.Table.Columns.Contains(prop.Name))
                                    {
                                        Type nullableUnderlyingType = Nullable.GetUnderlyingType(prop.PropertyType);
                                        var typedValue = (null != nullableUnderlyingType) ?
                                                Convert.ChangeType(dr[prop.Name], nullableUnderlyingType) :
                                                Convert.ChangeType(dr[prop.Name], prop.PropertyType);
                                        prop.SetValue(returnObj, typedValue, null);
                                    }
                                }
                            }

                            typedDataList.Add(returnObj);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.ErrorException = exp;
                result.Message = String.Format("Exception {0} in DataClient::GetTypedData method. See exception stack trace for more details.", exp.Message);
                result.IsOperationalError = true;
            }

            if (false == result.IsSuccess)
            {
                //TODO: Logging Result
            }
            return (result);
        }

        /// <summary>
        /// Call a stored procedure and get a single data value
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="scalerValueObject">The scaler value object.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="sqlServerAliasName">Name of the SQL server alias.</param>
        /// <param name="tableParams">Table Valued Parameters that don't get serialized as part of objects</param>
        /// <returns></returns>
        public DataClientResult GetScalarData(String storedProcedureName,
                                            out Object scalerValueObject,
                                            Dictionary<String, Object> parameters = null,
                                            String sqlServerAliasName = null, Dictionary<string, DataTable> tableParams = null)
        {
            DataClientResult result = new DataClientResult();
            scalerValueObject = null;

            try
            {
                List<DataTable> dataTablesList = null;
                result = CallDataServer(storedProcedureName, parameters, sqlServerAliasName, out dataTablesList, false, tableParams);

                if (result.IsSuccess && null != dataTablesList && dataTablesList.Count > 0 && dataTablesList[0].Rows.Count > 0)
                {
                    scalerValueObject = dataTablesList[0].Rows[0][0];
                }
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.ErrorException = exp;
                result.Message = String.Format("Exception {0} in DataClient::GetScalarData method. See exception stack trace for more details.", exp.Message);
                result.IsOperationalError = true;
            }

            if (false == result.IsSuccess)
            {
                //TODO: Logging Result
            }

            return (result);
        }

        /// <summary>
        /// Call a stored procedure and get DataTable list.
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="dataTableList">The data table list.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="sqlServerAliasName">Name of the SQL server alias.</param>
        /// <param name="tableParams">Table Valued Parameters that don't get serialized as part of objects</param>
        /// <returns></returns>
        public DataClientResult GetData(String storedProcedureName, out List<DataTable> dataTableList, Dictionary<String, Object> parameters = null, String sqlServerAliasName = null,
            Dictionary<string, DataTable> tableParams = null)
        {
            DataClientResult result = new DataClientResult();
            dataTableList = new List<DataTable>();

            try
            {
                result = CallDataServer(storedProcedureName, parameters, sqlServerAliasName, out dataTableList, false, tableParams);
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.ErrorException = exp;
                result.Message = String.Format("Exception {0} in DataClient::GetData method. See exception stack trace for more details.", exp.Message);
                result.IsOperationalError = true;
            }
            if (false == result.IsSuccess)
            {
                //TODO: Logging Result
            }

            return (result);
        }

        /// <summary>
        /// uses Data reader to pull the details from database
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="dataTableList"></param>
        /// <param name="parameters"></param>
        /// <param name="sqlServerAliasName"></param>
        /// <param name="tableParams"></param>
        /// <returns></returns>
        public DataClientResult GetDataByDataReader(String storedProcedureName,
           out List<DataTable> dataTableList,
           Dictionary<String, Object> parameters = null,
           String sqlServerAliasName = null, Dictionary<string, DataTable> tableParams = null)
        {
            DataClientResult result = new DataClientResult();
            dataTableList = new List<DataTable>();

            try
            {
                result = CallDataServer(storedProcedureName, parameters, sqlServerAliasName, out dataTableList, false, tableParams, true);
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.ErrorException = exp;
                result.Message = String.Format("Exception {0} in DataClient::GetData method. See exception stack trace for more details.", exp.Message);
                result.IsOperationalError = true;
            }
            if (false == result.IsSuccess)
            {
                //TODO: Logging Result
            }

            return (result);
        }

        /// <summary>
        /// Call a stored procedure to only update the data in DB, returns rows affected
        /// If proc has SET NOCOUNT ON, rows affected value returned will be -1
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="rowsAffected">The rows affected. If proc has SET NOCOUNT ON, rows affected value returned will be -1</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="sqlServerAliasName">Name of the SQL server alias.</param>
        /// <param name="tableParams">Table Valued Parameters that don't get serialized as part of objects</param>
        /// <returns></returns>
        public DataClientResult SetData(String storedProcedureName, out Int32 rowsAffected, Dictionary<String, Object> parameters = null, String sqlServerAliasName = null,
            Dictionary<string, DataTable> tableParams = null)
        {
            rowsAffected = -1;
            DataClientResult result = new DataClientResult();
            List<DataTable> dataTableList = new List<DataTable>();

            try
            {
                result = CallDataServer(storedProcedureName, parameters, sqlServerAliasName, out dataTableList, true, tableParams);
                if (result.IsSuccess && dataTableList != null && dataTableList.Count > 0 && dataTableList[0].Rows.Count > 0)
                {
                    rowsAffected = Convert.ToInt32(dataTableList[0].Rows[0][0]);
                }
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.ErrorException = exp;
                result.Message = String.Format("Exception {0} in DataClient::SetData method. See exception stack trace for more details.", exp.Message);
                result.IsOperationalError = true;
            }
            if (false == result.IsSuccess)
            {
                //TODO: Logging Result
            }

            return (result);
        }

        #endregion

        #region Private

        private DataClientResult CallDataServer(string storedProcedureName, Dictionary<String, object> parameters, string sqlServerAliasName, out List<DataTable> dataTablesList,
            Boolean callSet = false, Dictionary<string, DataTable> tableParams = null, bool isDataReader = false)
        {
            DataClientResult result = new DataClientResult();
            dataTablesList = new List<DataTable>();

            IDataServer dataServer = new DataServer();
            if (callSet)
            {
                DataTable setDataTable = dataServer.SetData(storedProcedureName, parameters, sqlServerAliasName, tableParams);
                dataTablesList.Add(setDataTable);
            }
            else if (isDataReader)
            {
                dataTablesList = dataServer.GetDataByDataReader(storedProcedureName, parameters, sqlServerAliasName,
                    tableParams);
            }
            else
            {
                dataTablesList = dataServer.GetData(storedProcedureName, parameters, sqlServerAliasName, tableParams);
            }

            if (null != dataTablesList)
            {
                if (dataTablesList.Count > 0 && dataTablesList[0].TableName == "PROC_CALL_ERROR")
                {
                    result.IsSuccess = false;
                    result.IsOperationalError = true;
                    result.Message = dataTablesList[0].Rows.Count > 0 ? dataTablesList[0].Rows[0][0].ToString() : String.Format("Error while calling {0} stored procedure! Check configuration and data access service for more details.", storedProcedureName);
                }
            }
            else
            {
                result.IsSuccess = false;
                result.IsOperationalError = true;
                result.Message = String.Format("NULL response received while calling {0} stored procedure! Check configuration and data access service for more details.", storedProcedureName);
            }

            return (result);
        }

        #endregion
    }
}
