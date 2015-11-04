using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfluxClient
{
    /// <summary>
    /// Implements a native container format for the InfluxDB measurements. 
    /// </summary>
    public class Measurement
    {
        /// <summary>
        /// The default constructor
        /// </summary>
        public Measurement()
        {
            //  Initialize the lists:
            Tags = new List<KeyValuePair<string, string>>();
            IntegerFields = new List<KeyValuePair<string, int>>();
            FloatFields = new List<KeyValuePair<string, float>>();
            BooleanFields = new List<KeyValuePair<string, bool>>();
            StringFields = new List<KeyValuePair<string, string>>();

            //  Initialize the timestamp
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// The measurement name
        /// </summary>
        public string Name
        { get; set; }

        /// <summary>
        /// Measurement tags
        /// </summary>
        public List<KeyValuePair<string, string>> Tags
        { get; set; }

        /// <summary>
        /// Adds a tag
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Measurement AddTag(string key, string value)
        {
            this.Tags.Add(new KeyValuePair<string, string>(key, value));

            return this;
        }

        /// <summary>
        /// A list of integer fields for this measurement
        /// </summary>
        public List<KeyValuePair<string, int>> IntegerFields
        { get; set; }

        /// <summary>
        /// A list of float fields for this measurement
        /// </summary>
        public List<KeyValuePair<string, float>> FloatFields
        { get; set; }

        /// <summary>
        /// A list of boolean fields for this measurement
        /// </summary>
        public List<KeyValuePair<string, bool>> BooleanFields
        { get; set; }

        /// <summary>
        /// A list of string fields for this measurement
        /// </summary>
        public List<KeyValuePair<string, string>> StringFields
        { get; set; }

        /// <summary>
        /// The timestamp for this measurement.  Defaults to UTC now
        /// </summary>
        public DateTime Timestamp
        { get; set; }
    }
}
