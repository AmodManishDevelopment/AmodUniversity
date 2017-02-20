using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amod.University.Data.Server
{
    public interface IDataServer
    {
        List<DataTable> GetData(String storedProcedureName, Dictionary<String, Object> parameters = null, String sqlServerAliasName = null, Dictionary<string, DataTable> tableParams = null);

        List<DataTable> GetDataByDataReader(String storedProcedureName, Dictionary<String, Object> parameters = null, String sqlServerAliasName = null, Dictionary<string, DataTable> tableParams = null);

        DataTable SetData(String storedProcedureName, Dictionary<String, object> parameters = null, String sqlServerAliasName = null, Dictionary<string, DataTable> tableParams = null);
    }
}
