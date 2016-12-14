using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Logic.Database;
using Newtonsoft.Json;

namespace Logic.DataLoading
{
    /// <summary>
    /// Class to load data about concerts using Kuda-go public api
    /// </summary>
    public class DataLoader
    {
        /// <summary>
        /// URL to perform request to Kuda-go api
        /// Documentation can be found at
        /// https://docs.kudago.com/api/#page:события,header:события-список-событий
        /// </summary>
        private string url = "https://kudago.com/public-api/v1.3/events/?lang=en" +
                             // search for concerts
                             "&categories=concert" +

                             // remove links from descriprions and names
                             "&text_format=text" +

                             // download these fields
                             "&fields=id,dates,title,description,location,price,site_url" +

                             // get date and time, not only ticks
                             "&expand=dates";

        /// <summary>
        /// Perform request to api 
        /// </summary>
        /// <returns>List of objects containing info about request</returns>
        private List<JsonRequest> GetJson()
        {
            WebClient client = new WebClient();
            List<JsonRequest> json = new List<JsonRequest>();

            // perform request to get first 100 concerts, decode in from UTF8
            string st = Encoding.UTF8.GetString(client.DownloadData($"{url}" +
                                                                    $"&page_size={100}" +
                                                                    $"&page={1}" +

                                                                    // get future concerts only
                                                                    $"&actual_since={DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}"));

            // deserialize aquired data
            JsonRequest jr = JsonConvert.DeserializeObject<JsonRequest>(st);
            json.Add(jr);

            // since result of each request contains a link to the next page, we can simply use it to continue requesting
            while (json.Last().next != null)
                json.Add(
                    JsonConvert.DeserializeObject<JsonRequest>(
                        Encoding.UTF8.GetString(client.DownloadData(json.Last().next))));

            return json;
        }

        /// <summary>
        /// Gets data and transforms it into database fields
        /// </summary>
        /// <returns>List of DbConcerts</returns>
        internal List<DbConcert> GetConcerts()
        {
            List<JsonRequest> jsons = GetJson();
            List<DbConcert> concerts = new List<DbConcert>();

            // convert result of each request into DbConcerts
            jsons.ForEach(json => { concerts.AddRange(json.Concerts); });
            return concerts;
        }
    }
}