using System;
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
            System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<Context, Migrations.Configuration>());
        }

        public DbSet<DbConcert> Concerts { get; set; }
        public DbSet<DbUser> Users { get; set; }
    }
}