﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Logic.DataLoading;

namespace Logic.Database
{
    /// <summary>
    /// Class to interact with database
    /// </summary>
    public class Context : DbContext
    {
        public Context() : base("localsql")
        {
            
        }

        internal DbSet<DbConcert> Concerts { get; set; }
        internal DbSet<DbUser> Users { get; set; }
    }
}