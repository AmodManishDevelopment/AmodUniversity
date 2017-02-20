using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Amod.University.Data.Client;
using Amod.University.Data.Helper;

namespace Amod.University.Data.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetDataTest()
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
    }
}
