using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace InfluxClient
{
    public class InfluxManager
    {
        #region Constructor and private properties

        /// <summary>
        /// The base url for API calls
        /// </summary>
        private string _baseUrl = "";

        /// <summary>
        /// The influxDB database to use
        /// </summary>
        private string _database = "";

        /// <summary>
        /// User name to use
        /// </summary>
        private string _username = "";

        /// <summary>
        /// Password to use
        /// </summary>
        private string _password = "";

        /// <summary>
        /// Creates a new InfluxDB manager
        /// </summary>
        /// <param name="influxEndpoint">The influxdb endpoint, including the port (if any)</param>
        /// <param name="database">The database to write to</param>
        public InfluxManager(string influxEndpoint, string database)
        {
            //  If the endpoint has a trailing backslash, remove it:
            if(influxEndpoint.EndsWith("/"))
            { influxEndpoint = influxEndpoint.Remove(influxEndpoint.LastIndexOf("/")); }

            //  Set the base url and database:
            _baseUrl = influxEndpoint;
            _database = database;
        }

        /// <summary>
        /// Creates a new InfuxDB manager with authentication credentials
        /// </summary>
        /// <param name="influxEndpoint">The influxdb endpoint, including the port (if any)</param>
        /// <param name="database">The database to write to</param>
        /// <param name="username">The username to authenticate with</param>
        /// <param name="password">The password to authenticate with</param>
        public InfluxManager(string influxEndpoint, string database, string username, string password) : this(influxEndpoint, database)
        {
            //  Set the username and password:
            _username = username;
            _password = password;
        }

        #endregion

        /// <summary>
        /// Pings the InfluxDB database
        /// </summary>
        /// <returns></returns>
        async public Task<HttpResponseMessage> Ping()
        {
            //  Create our url to ping
            string url = string.Format("{0}/ping", _baseUrl);

            //  Make an async call to get the response
            using(HttpClient client = new HttpClient())
            {
                return await client.GetAsync(url);
            }
        }

        /// <summary>
        /// Write a measurement to the InfluxDB database
        /// </summary>
        /// <param name="m">The measurement to write.  It must have at least one field specified</param>
        /// <returns>An awaitable Task containing the HttpResponseMessage returned from the InfluxDB server</returns>
        async public Task<HttpResponseMessage> Write(Measurement m)
        {
            //  Make sure the measurement has at least one field:
            if(!(m.BooleanFields.Any()
                || m.FloatFields.Any()
                || m.IntegerFields.Any()
                || m.StringFields.Any()))
            {
                throw new ArgumentException(string.Format("Measurement '{0}' needs at least one field value", m.Name));
            }

            //  Create our url to post data to
            string url = string.Format("{0}/write?db={1}", _baseUrl, _database);

            //  Create our data to post:
            HttpContent content = new StringContent(LineProtocol.Format(m));

            //  Make an async call to get the response
            using(HttpClient client = new HttpClient())
            {
                if(CredentialsHaveBeenSet())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetHttpBasicAuthCredentials());
                }
                return await client.PostAsync(url, content);
            }
        }

        /// <summary>
        /// Writes a list of measurements to the InfluxDB database
        /// </summary>
        /// <param name="listOfMeasurements">The list of measurements to write.  Each measurement must have at least one field specified</param>
        /// <returns>An awaitable Task containing the HttpResponseMessage returned from the InfluxDB server</returns>
        async public Task<HttpResponseMessage> Write(List<Measurement> listOfMeasurements)
        {
            //  Create our url to post data to
            string url = string.Format("{0}/write?db={1}", _baseUrl, _database);

            //  Our string to build:
            StringBuilder sb = new StringBuilder();
            foreach(var m in listOfMeasurements)
            {
                //  Make sure the measurement has at least one field:
                if(!(m.BooleanFields.Any()
                    || m.FloatFields.Any()
                    || m.IntegerFields.Any()
                    || m.StringFields.Any()))
                {
                    throw new ArgumentException(string.Format("Measurement '{0}' needs at least one field value", m.Name));
                }

                sb.AppendFormat("{0}\n", LineProtocol.Format(m));
            }

            //  If we had some measurements... 
            if(listOfMeasurements.Any())
            {
                //  Remove the last trailing newline
                sb.Remove(sb.Length - 1, 1);
            }

            //  Create our data to post:
            HttpContent content = new StringContent(sb.ToString());

            //  Make an async call to get the response
            using(HttpClient client = new HttpClient())
            {
                if(CredentialsHaveBeenSet())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetHttpBasicAuthCredentials());
                }
                return await client.PostAsync(url, content);
            }
        }

        #region API helpers

        /// <summary>
        /// Gets InfluxDB credentials in HTTP basic auth format 
        /// if they have been set.  Returns an empty string if they have not
        /// </summary>
        /// <returns></returns>
        private string GetHttpBasicAuthCredentials()
        {
            string retval = string.Empty;

            //  If the username and password aren't empty ... 
            if(CredentialsHaveBeenSet())
            {
                //  ... Format the username/password string
                byte[] byteArray = Encoding.ASCII.GetBytes(
                    string.Format("{0}:{1}", _username, _password)
                    );

                //  base 64 encode the string
                retval = Convert.ToBase64String(byteArray);
            }            

            return retval;
        }

        /// <summary>
        /// Returns 'true' if credentials have been set, false if they haven't
        /// </summary>
        /// <returns></returns>
        private bool CredentialsHaveBeenSet()
        {
            bool retval = false;

            if(!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
            {
                retval = true;
            }

            return retval;
        }

        #endregion
    }
}
