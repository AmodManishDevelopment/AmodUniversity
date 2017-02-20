using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amod.University.Data.Helper
{
    /// <summary>
    /// The Class that holds filters for a table and its parent table
    /// </summary>
    public class DataTableFilterParams
    {
        /// <summary>
        /// The Index in the ordinal reference of a datalist or a dataset
        /// </summary>
        public int OrdinalIndex { get; set; }

        /// <summary>
        /// Filters the Data table by the Column name 
        /// </summary>
        public string FilterByColumnName { get; set; }

        /// <summary>
        /// Parent Table Index that the value need to be referenced
        /// </summary>
        public int ParentTableIndex { get; set; }
    }
}
