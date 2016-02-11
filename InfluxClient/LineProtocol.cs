using System;
using System.Text;

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
            foreach(var tag in m.Tags)
            {
                retval.AppendFormat(",{0}={1}", Escape(tag.Name), Escape(tag.Value));
            }

            //  Append a space:
            retval.Append(" ");

            //  If we have any fields (we should have at least one), append them:
            //  BOOLEAN FIELDS
            foreach(var boolField in m.BooleanFields)
            {
                retval.AppendFormat("{0}={1},", Escape(boolField.Name), Convert.ToString(boolField.Value).ToLower());
            }

            //  FLOAT FIELDS
            foreach(var floatField in m.FloatFields)
            {
                retval.AppendFormat("{0}={1},", Escape(floatField.Name), floatField.Value);
            }

            //  INTEGER FIELDS
            foreach(var intField in m.IntegerFields)
            {
                retval.AppendFormat("{0}={1}i,", Escape(intField.Name), intField.Value);
            }

            //  STRING FIELDS
            foreach(var stringField in m.StringFields)
            {
                retval.AppendFormat("{0}=\"{1}\",", Escape(stringField.Name), EscapeString(stringField.Value));
            }

            //  Remove the last trailing comma
            retval.Remove(retval.Length - 1, 1);

            //  Append the timestamp (in nanoseconds past the epoch):
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);        
            retval.AppendFormat(" {0}", Convert.ToInt64((m.Timestamp.ToUniversalTime() - epoch).TotalSeconds * 1000000000));

            return retval.ToString();
        }
 
        /// <summary>
        /// Escapes strings properly for the line protocol
        /// </summary>
        /// <param name="stringToEscape">The string to escape</param>
        /// <returns></returns>
        private static string Escape(string stringToEscape)
        {
            string retval = stringToEscape;

            //  Escape spaces
            retval = retval.Replace(" ", @"\ ");

            //  Escape commas
            retval = retval.Replace(",", @"\,");

            //  Escape equal sign
            retval = retval.Replace("=", @"\=");

            return retval;
        }

        /// <summary>
        /// Escapes string values properly for the line protocol
        /// </summary>
        /// <param name="stringToEscape"></param>
        /// <returns></returns>
        private static string EscapeString(string stringToEscape)
        {
            string retval = stringToEscape;

            //  Escape double quotes
            retval = retval.Replace("\"", "\\\"");

            return retval;
        }
    }
}
