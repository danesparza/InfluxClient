using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using InfluxClient.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfluxClient.Tests
{
    [TestClass]
    public class ManagerTests
    {
        string influxEndpoint = "http://YOURSERVER:8086/";
        string influxDatabase = "YOURDATABASE";

        /// <summary>
        /// This test just verifies the field arguments.  It shouldn't matter
        /// what endpoint/database you pass to it.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Should have complained about the lack of fields")]
        public async Task Write_WithNoMeasurementFields_ThrowsException()
        {
            //  Arrange
            InfluxManager mgr = new InfluxManager(influxEndpoint, influxDatabase);

            Measurement m = new Measurement()
            {
                Name = "unittest"
            };

            //  Act
            var retval = await mgr.Write(m);

            //  Assert
            Assert.IsNull(retval);
        }

        /// <summary>
        /// You can run this as an integration test against your own server
        /// (Just uncomment the [TestMethod] attribute and rebuild)
        /// </summary>
        /// <returns></returns>
        // [TestMethod]
        public async Task Write_WithValidMeasurementFields_IsSuccessful()
        {
            //  Arrange
            InfluxManager mgr = new InfluxManager(influxEndpoint, influxDatabase);
            Measurement m = new Measurement()
            {
                Name = "unittest",
                IntegerFields = new List<Fields.IntegerField>()
                {
                    new IntegerField() { Name="count", Value=44 }
                },
                Timestamp = DateTime.Parse("10/26/2015 13:48")
            };

            //  Act
            Task<HttpResponseMessage> asyncretval = mgr.Write(m);
            Debug.WriteLine(DateTime.Now); // Log the time right after the call:

            HttpResponseMessage retval = await asyncretval; // Await the return
            Debug.WriteLine(DateTime.Now); // Log the time right after the return:

            //  Assert
            Assert.IsNotNull(retval);
            Assert.AreEqual(204, (int)retval.StatusCode);
        }

        /// <summary>
        /// You can run this as an integration test against your own server
        /// (Just uncomment the [TestMethod] attribute and rebuild)
        /// </summary>
        /// <returns></returns>
        // [TestMethod]
        public async Task Write_WithValidMeasurementFieldsNoTimestamp_IsSuccessful()
        {
            //  Arrange
            InfluxManager mgr = new InfluxManager(influxEndpoint, influxDatabase);
            Measurement m = new Measurement()
            {
                Name = "unittest",
                IntegerFields = new List<Fields.IntegerField>()
                {
                    new IntegerField() { Name="count", Value=88 }
                }
            };

            //  Act
            Task<HttpResponseMessage> asyncretval = mgr.Write(m);
            Debug.WriteLine(DateTime.Now); // Log the time right after the call:

            HttpResponseMessage retval = await asyncretval; // Await the return
            Debug.WriteLine(DateTime.Now); // Log the time right after the return:

            //  Assert
            Assert.IsNotNull(retval);
            Assert.AreEqual(204, (int)retval.StatusCode);
        }
    }
}
