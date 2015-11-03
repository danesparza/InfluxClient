using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace InfluxClient
{
    public class InfluxManager
    {
        /// <summary>
        /// The base url for API calls
        /// </summary>
        private string _baseUrl = "";

        /// <summary>
        /// The influxDB database to use
        /// </summary>
        private string _database = "";

        /// <summary>
        /// Creates a new Influx manager
        /// </summary>
        /// <param name="influxEndpoint">The influxdb endpoint, including the port (if any)</param>
        /// <param name="database">The database to write to</param>
        public InfluxManager(string influxEndpoint, string database)
        {
            _baseUrl = influxEndpoint;
            _database = database;
        }

        async private Task<HttpResponseMessage> Write(object dataToWrite)
        {
            HttpClient client = new HttpClient();

            //  Create our url to post data to
            string url = string.Format("{0}/write?db={1}", _baseUrl, _database);

            //  Create our data to post:
            //  We were calling serialize, but the JSON protocol is depcrecated.  We should
            //  be using the line protocol: https://influxdb.com/docs/v0.9/write_protocols/line.html
            // HttpContent content = new StringContent(Serialize(dataToWrite));

            //  Make an async call to get the response
            return await client.PostAsync(url, content);
        }

        #region API helpers

        /// <summary>
        /// Makes an API call and deserializes return value to the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiCall"></param>
        /// <returns></returns>
        async private Task<T> MakeAPICall<T>(string apiCall)
        {
            HttpClient client = new HttpClient();

            //  Make an async call to get the response
            var objString = await client.GetStringAsync(apiCall).ConfigureAwait(false);

            //  Deserialize and return
            return Deserialize<T>(objString);
        }

        #endregion
    }
}
