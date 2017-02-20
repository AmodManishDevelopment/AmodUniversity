using Amod.University.Data.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amod.University.Data.Client
{
    public interface IDataClient<T>
    {
        /// <summary>
        /// Gets Multi typed data set
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="typesOfObjectsToReturn"></param>
        /// <param name="typeResultObjectsDictionary"></param>
        /// <param name="parameters"></param>
        /// <param name="sqlServerAliasName"></param>
        /// <param name="tableParams"></param>
        /// <returns></returns>
        DataClientResult GetMultiTypedData(String storedProcedureName,
            List<Type> typesOfObjectsToReturn,
            out Dictionary<Type, List<Object>> typeResultObjectsDictionary,
            Dictionary<String, Object> parameters = null,
            String sqlServerAliasName = null, Dictionary<string, DataTable> tableParams = null);

        /// <summary>
        /// Get Typed Data
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="typedDataList"></param>
        /// <param name="parameters"></param>
        /// <param name="sqlServerAliasName"></param>
        /// <param name="tableParams"></param>
        /// <returns></returns>
        DataClientResult GetTypedData(String storedProcedureName,
            out List<T> typedDataList,
            Dictionary<String, Object> parameters = null,
            String sqlServerAliasName = null, Dictionary<string, DataTable> tableParams = null);

        /// <summary>
        /// Get Data
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="dataTableList"></param>
        /// <param name="parameters"></param>
        /// <param name="sqlServerAliasName"></param>
        /// <param name="tableParams"></param>
        /// <returns></returns>
        DataClientResult GetData(String storedProcedureName,
            out List<DataTable> dataTableList,
            Dictionary<String, Object> parameters = null,
            String sqlServerAliasName = null, Dictionary<string, DataTable> tableParams = null);

        /// <summary>
        /// Gets Data by DataReader
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="dataTableList"></param>
        /// <param name="parameters"></param>
        /// <param name="sqlServerAliasName"></param>
        /// <param name="tableParams"></param>
        /// <returns></returns>
        DataClientResult GetDataByDataReader(String storedProcedureName,
           out List<DataTable> dataTableList,
           Dictionary<String, Object> parameters = null,
           String sqlServerAliasName = null, Dictionary<string, DataTable> tableParams = null);

        /// <summary>
        /// Sets data
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="rowsAffected"></param>
        /// <param name="parameters"></param>
        /// <param name="sqlServerAliasName"></param>
        /// <param name="tableParams"></param>
        /// <returns></returns>
        DataClientResult SetData(String storedProcedureName,
            out Int32 rowsAffected,
            Dictionary<String, object> parameters = null,
            String sqlServerAliasName = null, Dictionary<string, DataTable> tableParams = null);

        /// <summary>
        /// Gets dscalr data
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="scalerValueObject"></param>
        /// <param name="parameters"></param>
        /// <param name="sqlServerAliasName"></param>
        /// <param name="tableParams"></param>
        /// <returns></returns>
        DataClientResult GetScalarData(String storedProcedureName,
            out Object scalerValueObject,
            Dictionary<String, Object> parameters = null,
            String sqlServerAliasName = null, Dictionary<string, DataTable> tableParams = null);
    }
}
