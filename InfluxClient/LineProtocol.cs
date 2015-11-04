using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfluxClient
{
    /// <summary>
    /// Utility class for the InfluxDb line protocol.  
    /// See https://influxdb.com/docs/v0.9/write_protocols/line.html for more information
    /// </summary>
    public class LineProtocol
    {
        public static string Format(Measurement m)
        {
            //  Example:
            //  "cpu_load_short,host=server01,region=us-west value=0.64";

            StringBuilder retval = new StringBuilder();

            //  Start with the measurement name:
            retval.AppendFormat("{0}", Escape(m.Name));

            //  If we have any tags, append them:
            foreach(var kvp in m.Tags)
            {
                retval.AppendFormat(",{0}={1}", Escape(kvp.Key), Escape(kvp.Value));
            }

            //  Append a space:
            retval.Append(" ");

            //  If we have any fields (we should have at least one), append them:
            //  BOOLEAN FIELDS
            foreach(var kvp in m.BooleanFields)
            {
                retval.AppendFormat("{0}={1},", Escape(kvp.Key), Convert.ToString(kvp.Value));
            }

            //  FLOAT FIELDS
            foreach(var kvp in m.FloatFields)
            {
                retval.AppendFormat("{0}={1},", Escape(kvp.Key), kvp.Value);
            }

            //  INTEGER FIELDS
            foreach(var kvp in m.IntegerFields)
            {
                retval.AppendFormat("{0}={1}i,", Escape(kvp.Key), kvp.Value);
            }

            //  STRING FIELDS
            foreach(var kvp in m.StringFields)
            {
                retval.AppendFormat("{0}=\"{1}\",", Escape(kvp.Key), Escape(kvp.Value));
            }

            //  Remove the last trailing comma
            retval.Remove(retval.Length - 1, 1);

            //  Append the timestamp (in micro-seconds past the epoch):
            retval.AppendFormat(" {0}", ((long)m.Timestamp.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds) * 1000);

            return retval.ToString();
        }

        /// <summary>
        /// Escapes strings properly for the line protocol
        /// </summary>
        /// <param name="stringToEscape">The string to escape</param>
        /// <returns></returns>
        public static string Escape(string stringToEscape)
        {
            string retval = stringToEscape;

            //  Escape spaces
            retval = retval.Replace(" ", @"\ ");

            //  Escape commas
            retval = retval.Replace(",", @"\,");

            return retval;
        }
    }
}
