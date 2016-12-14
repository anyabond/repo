using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Logic.Database
{
    /// <summary>
    /// Class represents user as field of DB
    /// </summary>
    public class DbUser : User
    {
        [Key]
        public int Id { get; set; }

        public virtual List<DbConcert> Wishlist { get; set; }
    }
}