using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfluxClient.Tests
{
    [TestClass]
    public class ManagerTests
    {
        string influxEndpoint = "http://your-server-here:8086/";
        string influxDatabase = "YOUR_DATABASE_HERE";

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Should have complained about the lack of fields")]
        public async Task Write_WithNoMeasurementFields_ThrowsException()
        {
            //  Arrange
            InfluxManager mgr = new InfluxManager(influxEndpoint, influxDatabase);

            Measurement m = new Measurement()
            {
                Name = "Unit test"
            };

            //  Act
            var retval = await mgr.Write(m);

            //  Assert
            Assert.IsNull(retval);
        }

        [TestMethod]
        public async Task Write_WithValidMeasurementFields_IsSuccessful()
        {
            //  Arrange
            InfluxManager mgr = new InfluxManager(influxEndpoint, influxDatabase);
            Measurement m = new Measurement();
            m.Name = "Unit test";

            //  Act
            var retval = await mgr.Write(m);

            //  Assert
            Assert.IsNull(retval);
        }
    }
}
