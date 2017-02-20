using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Amod.University.Data.Helper
{
    public static class DataMapperHelper
    {
        private static readonly Dictionary<Type, PropertyInfo[]> PropertiesDictionaryCache = new Dictionary<Type, PropertyInfo[]>(2000);
        
        /// <summary>
        /// Convert any IEnumerable of strong type to DataTable,
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable<T>(this IEnumerable<T> data)
        {
            PropertyDescriptorCollection props =
                  TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable(typeof(T).Name);
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    table.Columns.Add(prop.Name, prop.PropertyType.GetGenericArguments()[0]);
                else
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            var values = new object[props.Count];
            foreach (var item in data)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        /// <summary>
        /// write Xml from DataTable
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static string WriteDataTableXml(this DataTable dataTable)
        {
            string returnXml;
            using (var writer = new StringWriter())
            {
                dataTable.WriteXml(writer, XmlWriteMode.WriteSchema, false);

                string dataTableXml = writer.ToString();
                returnXml = dataTableXml;
            }
            return returnXml;

        }

        /// <summary>
        /// read Xml and get a DataTable
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDatTableFromXml(this string dataTableXml)
        {
            var newDataTable = new DataTable();
            using (var stringReader = new StringReader(dataTableXml))
            {
                newDataTable.ReadXml(stringReader);
            }
            return newDataTable;

        }

        /// <summary>
        /// Compresses dataset and returns bytes.
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public static Byte[] CompressData(this DataSet dataset)
        {
            Byte[] data;
            using (var mem = new MemoryStream())
            {
                using (var deflate = new DeflateStream(mem, CompressionMode.Compress, true))
                {
                    var bFormat = new BinaryFormatter();
                    bFormat.Serialize(deflate, dataset);
                    deflate.Close();
                    data = mem.ToArray();
                    mem.Close();
                }

            }

            return data;
        }

        /// <summary>
        /// Decompresses byte array and returns DataSet.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataSet DecompressData(Byte[] data)
        {
            DataSet dataset;
            using (var mem = new MemoryStream(data))
            {
                using (var deflate = new DeflateStream(mem, CompressionMode.Decompress, true))
                {
                    data = Read(deflate);
                    using (var deCompressedStream = new MemoryStream(data))
                    {
                        dataset = new DataSet();
                        var bFormat = new BinaryFormatter();
                        dataset.RemotingFormat = SerializationFormat.Binary;
                        dataset = (DataSet)bFormat.Deserialize(mem);
                        deCompressedStream.Close();
                        deflate.Close();
                        mem.Close();
                    }

                }

            }
            return dataset;
        }

        #region Private Methods used by MapResultType method declared above.

        /// <summary>
        /// Gets the List of Complex Types
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="nameToDataTableMapper"></param>
        /// <param name="dtList"></param>
        /// <param name="parentRow"></param>
        /// <returns></returns>
        private static object GetListPopulated(PropertyInfo prop, Dictionary<string, DataTableFilterParams> nameToDataTableMapper, List<DataTable> dtList, DataRow parentRow)
        {
            DataTableFilterParams representation;
            if (!nameToDataTableMapper.TryGetValue(prop.Name, out representation))
            {
                representation = new DataTableFilterParams
                {
                    OrdinalIndex = 1,
                    FilterByColumnName = null
                };
            }
            Type[] genericArguments = prop.PropertyType.GetGenericArguments();
            Type type = typeof(List<>).MakeGenericType(genericArguments);
            var returnObjList = (IList)Activator.CreateInstance(type);
            var rowCollection = FilterDataTableBy(dtList[representation.OrdinalIndex], BuildDataTableFilterQueryExpression(dtList, representation, parentRow));

            int rowIndex = 0;

            foreach (DataRow itemRow in rowCollection)
            {
                var returnObj = Activator.CreateInstance(genericArguments[0]);
                DataRow row = itemRow;
                int index = rowIndex;

                Array.ForEach(genericArguments[0].GetProperties(), add =>
                {
                    if (add.PropertyType.IsGenericType &&
                                add.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        if (add.CanWrite)
                        {
                            var internalResultValue = GetListPopulated(add, nameToDataTableMapper, dtList, row);
                            add.SetValue(returnObj, internalResultValue, null);
                        }

                    }
                    else
                    {
                        if ((add.PropertyType.IsClass && add.PropertyType.Module.ScopeName != "CommonLanguageRuntimeLibrary"))
                        {
                            var retCompObj = GetComplexObject(add, nameToDataTableMapper, dtList, row);
                            if (add.CanWrite)
                            {
                                add.SetValue(returnObj, retCompObj, null);
                            }
                        }
                        else
                        {
                            if (!add.CanWrite || !row.Table.Columns.Contains(add.Name)) return;
                            var typedValue = SetSimpleValue(add, row);
                            add.SetValue(returnObj, typedValue, null);
                        }
                    }

                });
                returnObjList.Add(returnObj);
                rowIndex++;
            }
            return returnObjList;
        }

        /// <summary>
        /// Gets the Complex Object.
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="nameToDataTableMapper"></param>
        /// <param name="dtList"></param>
        /// <param name="homeRow"></param>
        /// <returns></returns>
        private static object GetComplexObject(PropertyInfo prop, Dictionary<string, DataTableFilterParams> nameToDataTableMapper, List<DataTable> dtList, DataRow homeRow)
        {
            DataTableFilterParams representation;
            if (!nameToDataTableMapper.TryGetValue(prop.Name, out representation))
            {
            }
            var returnObj = Activator.CreateInstance(prop.PropertyType);

            var dataRow = null == representation
                            ? homeRow
                            : FilterDataTableBy(dtList[representation.OrdinalIndex], BuildDataTableFilterQueryExpression(dtList, representation, homeRow)).FirstOrDefault();
            if (null == dataRow)
                return returnObj;
            Array.ForEach(prop.PropertyType.GetProperties(),add =>
            {
                if ((add.PropertyType.IsGenericType &&
                   add.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                {
                    var returnObjList = GetListPopulated(add, nameToDataTableMapper, dtList, dataRow);
                    if (add.CanWrite)
                    {
                        add.SetValue(returnObj, returnObjList, null);
                    }
                }
                else
                {
                    if (add.PropertyType.IsClass &&
                        add.PropertyType.Module.ScopeName != "CommonLanguageRuntimeLibrary")
                    {
                        var retCompObj = GetComplexObject(add, nameToDataTableMapper, dtList, dataRow);
                        if (add.CanWrite)
                        {
                            add.SetValue(returnObj, retCompObj, null);
                        }

                    }
                    else
                    {
                        if (add.CanWrite && dataRow.Table.Columns.Contains(add.Name))
                        {
                            var typedValue = SetSimpleValue(add, dataRow);
                            add.SetValue(returnObj, typedValue, null);
                        }
                    }
                }

            });
            return returnObj;
        }

        /// <summary>
        /// Gets the simple value for a property
        /// </summary>
        /// <param name="add"></param>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        private static object SetSimpleValue(PropertyInfo add, DataRow dataRow)
        {
            if (null == dataRow)
                return null;
            Type nullableUnderlyingType = Nullable.GetUnderlyingType(add.PropertyType);
            var typedValue = (null != nullableUnderlyingType) ?
                Convert.ChangeType(dataRow[add.Name] == DBNull.Value && nullableUnderlyingType.IsValueType ? Activator.CreateInstance(nullableUnderlyingType) : dataRow[add.Name], nullableUnderlyingType) :
                    Convert.ChangeType(dataRow[add.Name] == DBNull.Value && add.PropertyType.IsValueType ? Activator.CreateInstance(add.PropertyType) : dataRow[add.Name], add.PropertyType);

            return typedValue;
        }

        /// <summary>
        /// filters the datatable by the predicate 
        /// </summary>
        /// <param name="dtList"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private static IEnumerable<DataRow> FilterDataTableBy(this DataTable dtList, Expression<Func<DataRow, bool>> predicate)
        {
            if (null != predicate && null != dtList)
                return dtList.AsEnumerable().Where(predicate.Compile());
            return new List<DataRow>();
        }

        /// <summary>
        /// Builds the DataTable Filter Query Expression for the given index and filterByColumnName 
        /// </summary>
        /// <param name="dtList"></param>
        /// <param name="representation"></param>
        /// <returns></returns>
        private static Expression<Func<DataRow, bool>> BuildDataTableFilterQueryExpression(this List<DataTable> dtList, DataTableFilterParams representation, DataRow row)
        {
            object valueToCheck = null;
            if (null == representation || dtList.Count == 0 || null == row)
                return null;
            string columnName = representation.FilterByColumnName;

            try
            {
                int rowIndex = dtList[representation.ParentTableIndex].Rows.IndexOf(row);

                if (null != dtList[representation.ParentTableIndex] &&
                    dtList[representation.ParentTableIndex].Rows.Count > 0 &&
                    !string.IsNullOrEmpty(representation.FilterByColumnName) &&
                    rowIndex >= 0)
                {
                    valueToCheck = Convert.ChangeType(dtList[representation.ParentTableIndex].Rows[rowIndex][representation.FilterByColumnName],
                                                            dtList[representation.ParentTableIndex].Rows[rowIndex][representation.FilterByColumnName].GetType());
                }
            }
            catch (Exception ex)
            {
                //TODO: Logging ex
            }

            Expression<Func<DataRow, bool>> wherequery = dr => dr[columnName] != DBNull.Value && Convert.ChangeType(dr[columnName], dr[columnName].GetType()).Equals(valueToCheck);
            wherequery = null == valueToCheck ? null : Expression.Lambda<Func<DataRow, bool>>(wherequery.Body, wherequery.Parameters);
            return wherequery;
        }

        /// <summary>
        /// Reads full stream and returns bytes
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static Byte[] Read(Stream str)
        {
            using (var mem = new MemoryStream())
            {
                Byte[] arr = new Byte[16];
                while (true)
                {
                    int rd = 0;
                    if ((rd = str.Read(arr, 0, arr.Length)) <= 0)
                        return mem.ToArray();
                    mem.Write(arr, 0, rd);
                }
            }

        }

        #endregion

    }
}
