using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.Database
{
    /// <summary>
    /// Class represents concert
    /// </summary>
    public class Concert
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string URL { get; set; }
    }
}
