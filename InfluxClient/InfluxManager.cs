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
            HttpContent content = new StringContent(Serialize(dataToWrite));

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

        /// <summary>
        /// Deserializes the JSON string to the given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objString"></param>
        /// <returns></returns>
        private T Deserialize<T>(string objString)
        {
            using(var stream = new MemoryStream(Encoding.Unicode.GetBytes(objString)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// Serialize an object to JSON
        /// </summary>
        /// <param name="objToSerialize"></param>
        /// <returns></returns>
        public string Serialize(object objToSerialize)
        {
            //  Our return value:
            string retval = string.Empty;

            using(var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(objToSerialize.GetType());
                serializer.WriteObject(stream, objToSerialize);
                stream.Flush();

                // The StreamReader will read from the current 
                // position of the MemoryStream which is currently 
                // set at the end of the string we just wrote to it. 
                // We need to set the position to 0 in order to read 
                // from the beginning.
                stream.Position = 0;
                var sr = new StreamReader(stream);
                retval = sr.ReadToEnd();
            }

            return retval;
        }

        #endregion
    }
}
