using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Logic.Database
{
    /// <summary>
    /// Class represents concert as field of DB
    /// </summary>
    public class DbConcert : Concert
    {
        [Key]
        public int dbId { get; set; }
        
        public virtual List<DbUser> Users { get; set; }

        public Concert ToConcert()
        {
            return new Concert
            {
                Date = Date,
                Description = Description,
                ID = ID,
                Location = Location,
                Price = Price,
                Title = Title,
                URL = URL
            };
        }
    }
}