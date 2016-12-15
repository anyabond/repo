using Logic.Database;
using Logic.DataLoading;

namespace Logic.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Logic.Database.Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Logic.Database.Context context)
        {
            if (context.Database.Exists()) return;
            DataLoader dl = new DataLoader();
            foreach (DbConcert concert in dl.GetConcerts())
            {
                context.Concerts.AddOrUpdate(x=>x.ID, concert);
            }
            context.SaveChanges();
        }
    }
}
