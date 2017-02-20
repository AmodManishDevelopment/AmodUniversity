using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Amod.University.Data.Client;
using Amod.University.Data.Helper;
using System.Collections.Generic;
using System.Data;

namespace Amod.University.Data.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetScalarDataTest()
        {
            IDataClient<TestData> dataClient = new DataClient<TestData>();
            Object scalarValueObject = null;
            DataClientResult result = dataClient.GetScalarData("GetScalarValue", out scalarValueObject);
            Assert.IsNotNull(scalarValueObject);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.IsOperationalError);
            Assert.IsFalse(result.IsFunctionalError);
            Assert.IsTrue(Convert.ToInt32(scalarValueObject) > 0);
        }

        [TestMethod]
        public void GetTypedDataTest()
        {
            IDataClient<Demo> dataClient = new DataClient<Demo>();
            List<Demo> testDataList = null;
            DataClientResult result = dataClient.GetTypedData("master.dbo.sp_who2", out testDataList);
            Assert.IsNotNull(testDataList);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.IsOperationalError);
            Assert.IsFalse(result.IsFunctionalError);
            Assert.IsTrue(testDataList.Count > 0);
            Assert.IsTrue(testDataList[0].SPID > 0);
            Assert.IsTrue(testDataList[0].Login.Length > 0);
            Assert.IsTrue(testDataList[0].HostName.Length > 0);
        }

        [TestMethod]
        public void GetDataNoParamsTest()
        {
            IDataClient<Object> dataClient = new DataClient<Object>();
            List<DataTable> testDataList = null;
            DataClientResult result = dataClient.GetData("master.dbo.sp_who2", out testDataList);
            Assert.IsNotNull(testDataList);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.IsOperationalError);
            Assert.IsFalse(result.IsFunctionalError);
            Assert.IsTrue(testDataList.Count > 0);
            Assert.IsTrue(testDataList[0].Rows.Count > 0);
        }

        [TestMethod]
        public void GetDataWithParamsTest()
        {
            IDataClient<Object> dataClient = new DataClient<Object>();
            List<DataTable> testDataList = null;
            Dictionary<String, Object> parameters = new Dictionary<String, Object>(2);
            parameters.Add("@Param", "This is a Test Param");
            DataClientResult result = dataClient.GetData("GetDataUsingParam", out testDataList, parameters);
            Assert.IsNotNull(testDataList);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.IsOperationalError);
            Assert.IsFalse(result.IsFunctionalError);
            Assert.IsTrue(testDataList.Count > 0);
            Assert.IsTrue(testDataList[0].Rows.Count > 0);
        }
    }

    public class Demo
    {
        public Int32 SPID { get; set; }

        public String Login { get; set; }

        public String HostName { get; set; }
    }
}
