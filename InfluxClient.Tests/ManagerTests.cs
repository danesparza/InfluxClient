using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfluxClient.Tests
{
    [TestClass]
    public class ManagerTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            //  Arrange
            var testVar = new
            {
                item = "A string item",
                number = 42
            };

            InfluxManager mgr = new InfluxManager("", "");

            //  Act
            string retval = mgr.Serialize(testVar);

            //  Assert
            Assert.IsFalse(string.IsNullOrEmpty(retval));
        }
    }
}
