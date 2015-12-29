using System;
using System.Collections.Generic;
using System.Configuration;
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
        private string _influxEndpoint = string.Empty;
        private string _influxDatabase = string.Empty;
        private string _influxUser = string.Empty;
        private string _influxPassword = string.Empty;

        [TestInitialize]
        public void TestInit()
        {
            //  Read in our core settings from config:
            _influxEndpoint = ConfigurationManager.AppSettings["influxEndpoint"];
            _influxDatabase = ConfigurationManager.AppSettings["influxDatabase"];
            _influxUser = ConfigurationManager.AppSettings["influxUser"];
            _influxPassword = ConfigurationManager.AppSettings["influxPassword"];
        }

        /// <summary>
        /// This test just verifies the field arguments.  It shouldn't matter
        /// what endpoint/database you pass to it.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        [TestCategory("Write")]
        [ExpectedException(typeof(ArgumentException), "Should have complained about the lack of fields")]
        public async Task Write_WithNoMeasurementFields_ThrowsException()
        {
            //  Arrange
            InfluxManager mgr = new InfluxManager(_influxEndpoint, _influxDatabase);

            Measurement m = new Measurement()
            {
                Name = "unittest"
            };

            //  Act
            var retval = await mgr.Write(m);

            //  Assert
            Assert.IsNull(retval);
        }

        [TestMethod]
        [TestCategory("Write")]
        public async Task Write_WithValidMeasurementFields_IsSuccessful()
        {
            //  Arrange
            InfluxManager mgr = new InfluxManager(_influxEndpoint, _influxDatabase);
            Measurement m = new Measurement()
            {
                Name = "unittest",
                IntegerFields = new List<IntegerField>()
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

        [TestMethod]
        [TestCategory("Write")]
        public async Task Write_WithCredentialsAndValidMeasurementFields_IsSuccessful()
        {
            //  Arrange
            InfluxManager mgr = new InfluxManager(_influxEndpoint, _influxDatabase, _influxUser, _influxPassword);
            Measurement m = new Measurement("unittest").AddField("count", 42);

            //  Act
            Task<HttpResponseMessage> asyncretval = mgr.Write(m);
            Debug.WriteLine(DateTime.Now); // Log the time right after the call:

            HttpResponseMessage retval = await asyncretval; // Await the return
            Debug.WriteLine(DateTime.Now); // Log the time right after the return:

            //  Assert
            Assert.IsNotNull(retval);
            Assert.AreEqual(204, (int)retval.StatusCode);
        }

        [TestMethod]
        [TestCategory("Write")]
        public async Task Write_WithValidMeasurementFieldsNoTimestamp_IsSuccessful()
        {
            //  Arrange
            InfluxManager mgr = new InfluxManager(_influxEndpoint, _influxDatabase);
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

        [TestMethod]
        [TestCategory("Write")]
        public async Task Write_WithMultipleValidMeasurementFieldsNoTimestamp_IsSuccessful()
        {
            //  Arrange
            InfluxManager mgr = new InfluxManager(_influxEndpoint, _influxDatabase);
            List<Measurement> measurements = new List<Measurement>()
            {
                new Measurement()
                {
                    Name = "unittest",
                    IntegerFields = new List<Fields.IntegerField>()
                    {
                        new IntegerField() { Name="count", Value=91 }
                    },
                    Tags = new List<Tag>()
                    {
                        new Tag() { Name="element", Value="1" }
                    }
                },
                new Measurement()
                {
                    Name = "unittest",
                    IntegerFields = new List<Fields.IntegerField>()
                    {
                        new IntegerField() { Name="count", Value=92 }
                    },
                    Tags = new List<Tag>()
                    {
                        new Tag() { Name="element", Value="2" }
                    }
                }
            };

            //  Act
            Task<HttpResponseMessage> asyncretval = mgr.Write(measurements);
            Debug.WriteLine("{0} - Right after the call", DateTime.Now); // Log the time right after the call:

            HttpResponseMessage retval = await asyncretval; // Await the return
            Debug.WriteLine("{0} - Right after the return", DateTime.Now); // Log the time right after the return:

            //  Assert
            Assert.IsNotNull(retval);
            Assert.AreEqual(204, (int)retval.StatusCode);
        }

        [TestMethod]
        [TestCategory("Ping")]
        public async Task Ping_IsSuccessful()
        {
            //  Arrange
            InfluxManager mgr = new InfluxManager(_influxEndpoint, _influxDatabase, _influxUser, _influxPassword);

            //  Act
            HttpResponseMessage retval = await mgr.Ping();
            
            //  Assert
            Assert.IsNotNull(retval);
            Assert.AreEqual(204, (int)retval.StatusCode);
            Assert.IsTrue(retval.Headers.Contains("X-Influxdb-Version"));
        }

        [TestMethod]
        [TestCategory("Query")]
        public async Task QueryJSON_WithValidQuery_ReturnsString()
        {
            //  Arrange
            InfluxManager mgr = new InfluxManager(_influxEndpoint, _influxDatabase, _influxUser, _influxPassword);
            string data = string.Empty;

            //  Act
            HttpResponseMessage retval = await mgr.QueryJSON("select * from unittest");
            data = await retval.Content.ReadAsStringAsync();

            //  Assert
            Assert.IsNotNull(retval);
            Assert.AreEqual(200, (int)retval.StatusCode);
            Assert.IsTrue(retval.Headers.Contains("X-Influxdb-Version"));

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Length > 0);
        }
    }
}
