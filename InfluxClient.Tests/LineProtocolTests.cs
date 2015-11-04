using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfluxClient.Tests
{
    [TestClass]
    public class LineProtocolTests
    {
        [TestMethod]
        public void Format_WithValidMeasurement_IsSuccessful()
        {
            //  Arrange
            Measurement m = new Measurement()
            {
                Name = "cpu",
            };
            m.AddTag("host", "server01").AddTag("region", "us-west");
            
            string expectedFormat = "";
            string retval = string.Empty;

            //  Act
            retval = LineProtocol.Format(m);

            //  Assert
            Assert.AreEqual<string>(expectedFormat, retval);
        }
    }
}
