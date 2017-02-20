using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amod.University.Data.Test
{
    public class TestData
    {
        public Int64 IAmIdentityCol { get; set; }
        public float NumericCol { get; set; }
        public Boolean BitCol { get; set; }
        public Int16 SmallIntCol { get; set; }
        public decimal DecimalCol { get; set; }
        public decimal SmallMoneyCol { get; set; }
        public Int32 IntCol { get; set; }
        public Int16 TinyIntCol { get; set; }
        public decimal MoneyCol { get; set; }
        public float FloatCol { get; set; }
        public float RealCol { get; set; }
        public DateTime DateCol { get; set; }
        public DateTimeOffset DatetimeOffsetCol { get; set; }
        public DateTime DateTime2Col { get; set; }
        public DateTime SmallDatetimeCol { get; set; }
        public DateTime DatetimeCol { get; set; }
        public TimeSpan TimeCol { get; set; }
        public String CharCol { get; set; }
        public String VarcharCol { get; set; }
        public String NcharCol { get; set; }
        public String NvarcharCol { get; set; }
        public Guid UniqueIdentifierCol { get; set; }
        public String XmlCol { get; set; }
        public Int32 ComputedCol1 { get; set; }
        public Int32 ComputedCol2 { get; set; }
        public DateTime DefaultCol { get; set; }
    }
}
