using System;
using System.Collections.Generic;
using InfluxClient.Fields;

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
            Tags = new List<Tag>();
            IntegerFields = new List<IntegerField>();
            FloatFields = new List<FloatField>();
            BooleanFields = new List<BooleanField>();
            StringFields = new List<StringField>();

            //  Initialize the timestamp
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// The measurement name
        /// </summary>
        public string Name
        { get; set; }

        /// <summary>
        /// The list of tags for this measurement
        /// </summary>
        public List<Tag> Tags
        { get; set; }

        /// <summary>
        /// The list of integer fields for this measurement
        /// </summary>
        public List<IntegerField> IntegerFields
        { get; set; }

        /// <summary>
        /// A list of float fields for this measurement
        /// </summary>
        public List<FloatField> FloatFields
        { get; set; }

        /// <summary>
        /// A list of boolean fields for this measurement
        /// </summary>
        public List<BooleanField> BooleanFields
        { get; set; }

        /// <summary>
        /// A list of string fields for this measurement
        /// </summary>
        public List<StringField> StringFields
        { get; set; }
        
        /// <summary>
        /// The timestamp for this measurement.  Defaults to UTC now
        /// </summary>
        public DateTime Timestamp
        { get; set; }

        #region Helper methods

        /// <summary>
        /// Adds a tag.  This call is chainable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Measurement AddTag(string name, string value)
        {
            this.Tags.Add(new Tag() { Name = name, Value = value });
            return this;
        }

        /// <summary>
        /// Adds an integer field.  This call is chainable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Measurement AddField(string name, int value)
        {
            this.IntegerFields.Add(new IntegerField() { Name = name, Value = value });
            return this;
        }

        /// <summary>
        /// Adds a float field. This call is chainable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Measurement AddField(string name, float value)
        {
            this.FloatFields.Add(new FloatField() { Name = name, Value = value });
            return this;
        }

        /// <summary>
        /// Adds a boolean field. This call is chainable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Measurement AddField(string name, bool value)
        {
            this.BooleanFields.Add(new BooleanField { Name = name, Value = value });
            return this;
        }

        /// <summary>
        /// Adds a string field. This call is chainable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Measurement AddField(string name, string value)
        {
            this.StringFields.Add(new StringField() { Name = name, Value = value });
            return this;
        }

        #endregion

    }
}
