using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

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
        /// Action to tun when there is an exception
        /// </summary>
        private Action<Exception, string, object[]> _exceptionHandler = (exception, s, values) => { };

        /// <summary>
        /// Creates a new InfluxDB manager
        /// </summary>
        /// <param name="influxEndpoint">The influxdb endpoint, including the port (if any)</param>
        /// <param name="database">The database to write to</param>
        /// <param name="throwExceptions">Whether or not to throw any exceptions for methods called on this instance</param>
        public InfluxManager(string influxEndpoint, string database, bool throwExceptions = false) 
            : this(influxEndpoint, database, DetermineDefaultExceptionHandler(throwExceptions))
        { 
        }

        /// <summary>
        /// Creates a new InfuxDB manager with authentication credentials
        /// </summary>
        /// <param name="influxEndpoint">The influxdb endpoint, including the port (if any)</param>
        /// <param name="database">The database to write to</param>
        /// <param name="username">The username to authenticate with</param>
        /// <param name="password">The password to authenticate with</param>
        /// <param name="throwExceptions">Whether or not to throw any exceptions for methods called on this instance</param>
        public InfluxManager(string influxEndpoint, string database, string username, string password, bool throwExceptions = false) 
            : this(influxEndpoint, database, username, password, DetermineDefaultExceptionHandler(throwExceptions))
        {
        }

        /// <summary>
        /// Creates a new InfluxDB manager
        /// </summary>
        /// <param name="influxEndpoint">The influxdb endpoint, including the port (if any)</param>
        /// <param name="database">The database to write to</param>
        /// <param name="exceptionHandler">Action to handle exceptions</param>
        public InfluxManager(string influxEndpoint, string database, Action<Exception, string, object[]> exceptionHandler)
        {
            //  If the endpoint has a trailing backslash, remove it:
            if (influxEndpoint.EndsWith("/"))
            { influxEndpoint = influxEndpoint.Remove(influxEndpoint.LastIndexOf("/")); }

            //  Set the base url and database:
            _baseUrl = influxEndpoint;
            _database = database;

            //  Set the bubble exceptions parameter:
            _exceptionHandler = exceptionHandler;
        }

        /// <summary>
        /// Creates a new InfuxDB manager with authentication credentials
        /// </summary>
        /// <param name="influxEndpoint">The influxdb endpoint, including the port (if any)</param>
        /// <param name="database">The database to write to</param>
        /// <param name="username">The username to authenticate with</param>
        /// <param name="password">The password to authenticate with</param>
        /// <param name="exceptionHandler">Action to handle exceptions</param>
        public InfluxManager(string influxEndpoint, string database, string username, string password,
            Action<Exception, string, object[]> exceptionHandler) : this(influxEndpoint, database, exceptionHandler)
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
            //  The default response message:
            HttpResponseMessage retval = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            //  Create our url to ping
            string url = string.Format("{0}/ping", _baseUrl);

            try
            {
                //  Make an async call to get the response
                using(HttpClient client = new HttpClient())
                {
                    retval = await client.GetAsync(url);
                }
            }
            catch(Exception ex)
            {
                Trace.TraceError("Ping {0} caused an exception: {1}", url, ex.Message);

                LogError(ex, "Ping {0} caused an exception: {1}", url, ex.Message);
            }

            return retval;
        }

        /// <summary>
        /// Write a measurement to the InfluxDB database
        /// </summary>
        /// <param name="m">The measurement to write.  It must have at least one field specified</param>
        /// <returns>An awaitable Task containing the HttpResponseMessage returned from the InfluxDB server</returns>
        async public Task<HttpResponseMessage> Write(Measurement m)
        {
            //  The default response message:
            HttpResponseMessage retval = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            //  Make sure the measurement has at least one field:
            if(!(m.BooleanFields.Any()
                || m.FloatFields.Any()
                || m.IntegerFields.Any()
                || m.StringFields.Any()))
            {
                string error = string.Format("Measurement '{0}' needs at least one field value", m.Name);
                Trace.TraceError(error);

                LogError(new ApplicationException(error), error);
            }

            //  Create our url to post data to
            string url = string.Format("{0}/write?db={1}", _baseUrl, _database);

            //  Create our data to post:
            HttpContent content = new StringContent(LineProtocol.Format(m));

            try
            {
                //  Make an async call to get the response
                using(HttpClient client = new HttpClient())
                {
                    if(CredentialsHaveBeenSet())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetHttpBasicAuthCredentials());
                    }
                    retval = await client.PostAsync(url, content);
                }
            }
            catch(Exception ex)
            {
                Trace.TraceError("Write {0} caused an exception: {1}", url, ex.Message);

                LogError(ex, "Write {0} caused an exception: {1}", url, ex.Message);
            }

            return retval;
        }

        /// <summary>
        /// Writes a list of measurements to the InfluxDB database
        /// </summary>
        /// <param name="listOfMeasurements">The list of measurements to write.  Each measurement must have at least one field specified</param>
        /// <returns>An awaitable Task containing the HttpResponseMessage returned from the InfluxDB server</returns>
        async public Task<HttpResponseMessage> Write(List<Measurement> listOfMeasurements)
        {
            //  The default response message:
            HttpResponseMessage retval = new HttpResponseMessage(HttpStatusCode.InternalServerError);

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
                    string error = string.Format("Measurement '{0}' needs at least one field value", m.Name);
                    Trace.TraceError(error);

                    LogError(new ArgumentException(error), "Measurement '{0}' needs at least one field value", m.Name);
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

            try
            {
                //  Make an async call to get the response
                using(HttpClient client = new HttpClient())
                {
                    if(CredentialsHaveBeenSet())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetHttpBasicAuthCredentials());
                    }
                    retval = await client.PostAsync(url, content);
                }
            }
            catch(Exception ex)
            {
                Trace.TraceError("Write (list) {0} caused an exception: {1}", url, ex.Message);

                LogError(ex, "Write (list) {0} caused an exception: {1}", url, ex.Message);
            }

            return retval;
        }

        /// <summary>
        /// Query the InfluxDB database
        /// </summary>
        /// <param name="influxQL"></param>
        /// <returns></returns>
        async public Task<HttpResponseMessage> QueryJSON(string influxQL)
        {
            //  Create our url to query data with
            string url = string.Format("{0}/query?db={1}&q={2}", _baseUrl, _database, influxQL);

            //  Make an async call to get the response
            using(HttpClient client = new HttpClient())
            {
                if(CredentialsHaveBeenSet())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetHttpBasicAuthCredentials());
                }
                return await client.GetAsync(url);
            }
        }

        /// <summary>
        /// Query the InfluxDB database and deserialize the returned data
        /// to the specified object format
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.  See https://influxdb.com/docs/v0.9/guides/querying_data.html for the shape this should take</typeparam>
        /// <param name="influxQL"></param>
        /// <returns></returns>
        async public Task<T> Query<T>(string influxQL)
        {
            //  The return value:
            T retval = default(T);

            //  Call the QueryJSON method to get the data back:
            HttpResponseMessage response = await QueryJSON(influxQL);
            string data = await response.Content.ReadAsStringAsync();

            //  Serialize the data to the requested object
            //  (it should take the shape of the returned JSON
            //  https://influxdb.com/docs/v0.9/guides/querying_data.html)
            JavaScriptSerializer jser = new JavaScriptSerializer();
            retval = jser.Deserialize<T>(data);
            
            return retval;
        }

        /// <summary>
        /// Query the InfluxDB database and deserialize the returned data
        /// </summary>
        /// <param name="influxQL"></param>
        /// <returns></returns>
        async public Task<QueryResponse> Query(string influxQL)
        {
            QueryResponse retval = await Query<QueryResponse>(influxQL);
            return retval;
        }

        private void LogError(Exception exception, string message, params object[] values)
        {
            _exceptionHandler(exception, message, values);
        }

        private static Action<Exception, string, object[]> DetermineDefaultExceptionHandler(bool throwExceptions)
        {
            if (throwExceptions)
            {
                return (exception, s, values) => { ExceptionDispatchInfo.Capture(exception).Throw(); };
            }

            return (exception, s, values) => { };
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
