using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Logic.DataLoading;

namespace Logic.Database
{
    /// <summary>
    /// Class to perform requests to database
    /// </summary>
    public class Repository
    {
        private Context context = new Context();
        private DbUser currentUser;

        /// <summary>
        /// Checks whether passed login was already used or not
        /// </summary>
        /// <param name="login">Login to be checked</param>
        /// <returns>true if used, false in not</returns>
        public bool LoginUsed(string login)
        {
            var users = context.Users.ToList();
            return users.Find(u => u.Login.Equals(login)) != null;
        }

        /// <summary>
        /// Performs login into account with passed login and password
        /// </summary>
        /// <param name="login">Login</param>
        /// <param name="password">Password</param>
        public void Login(string login, string password)
        {
            var users = context.Users;
            DbUser dbUser = users.ToList().Find(u => login.Equals(u.Login));

            if (dbUser == null)
                throw new Exception("Login does not exist!");

            if (!dbUser.Password.Equals(password))
                throw new Exception("Wrong password!");

            currentUser = dbUser;
        }

        /// <summary>
        /// Performs registration of new user
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="lname">Surname</param>
        /// <param name="login">Login</param>
        /// <param name="password">Password</param>
        /// <param name="email">Email</param>
        public void Register(string name, string lname, string login, string password, string email)
        {
            if (LoginUsed(login))
                throw new Exception("Login was already used");

            DbUser dbUser = new DbUser
            {
                Name = name,
                Surname = lname,
                Login = login,
                Password = password,
                Email = email
            };

            // add new user to database
            context.Users.Add(dbUser);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets dictionary co convert long names to short names of the cities (to make proper requests to database)
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetCityShortingDictionary()
        {
            return GetShortingCityDictionary().ToList().ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }

        /// <summary>
        /// Gets dictionary to convert short names of cities (which provided by api) to long names
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetShortingCityDictionary()
        {
            return new Dictionary<string, string>
            {
                {"spb", "Санкт-Петербург"},
                {"msk", "Москва"},
                {"nsk", "Новосибирск"},
                {"ekb", "Екатеринбург"},
                {"nnv", "Нижний Новгород"},
                {"kzn", "Казань"},
                {"vbg", "Выборг"},
                {"smr", "Самара"},
                {"krd", "Краснодар"},
                {"sochi", "Сочи"},
                {"ufa", "Уфа"},
                {"krasnoyarsk", "Красноярск"},
                {"kev", "Киев"},
                {"new-york", "Нью-Йорк"},
                {"london", "Лондон"}
            };
        }

        /// <summary>
        /// Gets concerts in passed city between dates
        /// </summary>
        /// <param name="city">City to find concerts</param>
        /// <param name="fr">Start of interval</param>
        /// <param name="to">End of interval</param>
        /// <returns></returns>
        public List<Concert> CityConcerstBetweenDates(string city, DateTime fr, DateTime to)
        {
            return (from conc in context.Concerts
                where (conc.Date >= fr && conc.Date <= to && conc.Location.Equals(city))
                orderby conc.Date
                select conc).ToList().ConvertAll(c => c.ToConcert());
        }

        /// <summary>
        /// Gets concerts in wishlist of current user
        /// </summary>
        /// <returns>Concerts in wishlist of current user</returns>
        public List<Concert> UserWishlist()
        {
            if (currentUser?.Wishlist != null)
                return currentUser.Wishlist.ConvertAll(c => c.ToConcert());

            return new List<Concert>();
        }

        /// <summary>
        /// Adds concert with passed id in user's wishlist
        /// </summary>
        /// <param name="concertid">id of concert to be added</param>
        public void AddToWishlist(int concertid)
        {
            if (currentUser.Wishlist.Find(c => c.ID == concertid) == null)
            {
                currentUser.Wishlist.Add(context.Concerts.ToList().Find(c => c.ID == concertid));
                context.SaveChanges();
            }
            else throw new Exception("Concert already in your wishlist!");
        }

        /// <summary>
        /// Removes concert with passed id from user's wishlist
        /// </summary>
        /// <param name="concertid"></param>
        public void RemoveFromWishlist(int concertid)
        {
            DbConcert concert = currentUser.Wishlist.Find(c => c.ID == concertid);
            if (concert != null)
            {
                currentUser.Wishlist.Remove(concert);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates DB with fresh data
        /// </summary>
        public void UpdateDb()
        {
            DataLoader dl = new DataLoader();

            // upload new concerts
            foreach (DbConcert concert in dl.GetConcerts())
            {
                context.Concerts.AddOrUpdate(x => x.ID, concert);
            }
            List<DbConcert> remove = new List<DbConcert>();

            // remove old concerts
            foreach (DbConcert contextConcert in context.Concerts)
            {
                if (contextConcert.Date < DateTime.Now)
                    remove.Add(contextConcert);
            }
            context.Concerts.RemoveRange(remove);
            context.SaveChanges();
        }
    }
}