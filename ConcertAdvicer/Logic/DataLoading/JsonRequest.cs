using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.Database;
using Newtonsoft.Json;

namespace Logic.DataLoading
{
    /// <summary>
    /// Class to store and convert results of api requests
    /// 
    /// result of api request is string looking like this
    /// 
    /// {
    ///     "count" : "",
    ///     "next" : "",
    ///     "results" : 
    ///         [
    ///             {
    ///                 "id" : "",
    ///                 "title" : "",
    ///                 "dates" : [{ "start_date" : "", "start_time" : ""}],
    ///                 "location" : { "slug" : "" },
    ///                 "description" : "",
    ///                 "price" : "",
    ///                 "site_url" : ""
    ///             }
    ///             . . .
    ///         ]
    /// }
    /// </summary>
    internal class JsonRequest
    {
        // Total count of concerts
        [JsonProperty("count")]
        public int count { get; set; }

        // Link to the next page of request
        [JsonProperty("next")]
        public string next { get; set; }

        // Property to convert recieved data to database fields
        public List<DbConcert> Concerts
        {
            get { return results.ConvertAll(r => r.ConvertToConcert()).ToList(); }
        }

        // List of concerts recieved from api
        [JsonProperty("results")]
        public List<JsonConcert> results { get; set; }

        /// <summary>
        /// Class representing concert recieved from api
        /// </summary>
        internal class JsonConcert
        {
            [JsonProperty("id")]
            public int id { get; set; }

            [JsonProperty("title")]
            public string title { get; set; }

            // Dates in the json represents as array of one element, so list it is
            [JsonProperty("dates")]
            public List<JsonDates> dates { get; set; }

            [JsonProperty("location")]
            public JsonLocation location { get; set; }

            [JsonProperty("description")]
            public string description { get; set; }

            [JsonProperty("price")]
            public string price { get; set; }

            [JsonProperty("site_url")]
            public string site_url { get; set; }

            /// <summary>
            /// Converts JsonConcert to database field
            /// </summary>
            /// <returns>DbConcert stored in this json Concert</returns>
            public DbConcert ConvertToConcert()
            {
                DbConcert c = new DbConcert
                {
                    ID = id,

                    // Date or time can be null, so it needs to be managed
                    Date = !string.IsNullOrEmpty(dates.First().start_date)
                        ? (DateTime?)
                        (DateTime.Parse(dates.First()?.start_date) + (!string.IsNullOrEmpty(dates.First().start_time)?TimeSpan.Parse(dates.First()?.start_time):new TimeSpan(0)))
                        : null,

                    Description = description ?? "",
                    Title = title ?? "",
                    Location = location.slug ?? "",
                    // Consider empty price as free event
                    Price = string.IsNullOrEmpty(price)?"0":price,
                    URL = site_url ?? ""
                };
                return c;
            }
        }

        /// <summary>
        /// Represents the city in which concert will be
        /// </summary>
        internal class JsonLocation
        {
            // short name of city
            public string slug { get; set; }
        }

        /// <summary>
        /// Represents date and time of concert
        /// </summary>
        internal class JsonDates
        {
            public string start_date { get; set; }
            public string start_time { get; set; }
        }
    }
}