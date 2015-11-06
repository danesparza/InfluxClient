using System;
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
                Timestamp = DateTime.Parse("10/26/2015 13:48Z")
            };
            m.AddTag("host", "server01").AddTag("region", "us-west");
            m.AddField("alarm", false).AddField("Message", "Testing messages");

            string expectedFormat = "cpu,host=server01,region=us-west alarm=false,Message=\"Testing messages\" 1445867280000000000";
            string retval = string.Empty;

            //  Act
            retval = LineProtocol.Format(m);

            //  Assert
            Assert.AreEqual<string>(expectedFormat, retval);
        }

        [TestMethod]
        public void Format_WithNoTags_IsSuccessful()
        {
            //  Arrange
            Measurement m = new Measurement()
            {
                Name = "cpu",
                Timestamp = DateTime.Parse("10/26/2015 13:48Z")
            };
            m.AddField("alarm", false).AddField("Message", "Testing messages");

            string expectedFormat = "cpu alarm=false,Message=\"Testing messages\" 1445867280000000000";
            string retval = string.Empty;

            //  Act
            retval = LineProtocol.Format(m);

            //  Assert
            Assert.AreEqual<string>(expectedFormat, retval);
        }
    }
}
