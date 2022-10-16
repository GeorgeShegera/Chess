using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    [Serializable]
    public class User
    {
        [JsonProperty("Login")]
        public string Login { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("Rating")]
        public int Rating { get; set; }

        public User(string login, string password, int rating)
        {
            Login = login;
            Password = password;
            Rating = rating;
        }
        public User(string login, string password)
        {
            Login = login;
            Password = password;
        }
        public User()
        {
            Login = "";
            Password = "";
            Rating = 0;
        }
        public User(User person)
        {
            Login = person.Login;
            Password = person.Password;
        }
        public bool Authorized()
        {
            return Login != "" && Password != "";
        }
        public void AddRating(int rating)
        {
            Rating += rating;
        }
        public void SubtractRating(int rating)
        {
            if (Rating < rating)
            {
                Rating = 0;
            }
            else
            {
                Rating -= rating;
            }
        }
        public static bool operator ==(User player1, User player2)
        {
            return player1.Login == player2.Login && player1.Password == player2.Password;
        }
        public static bool operator !=(User player1, User player2)
        {
            return player1.Login != player2.Login || player1.Password != player2.Password;
        }
    }
}
