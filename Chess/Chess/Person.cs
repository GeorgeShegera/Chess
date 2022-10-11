using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    [Serializable]
    public class Person
    {
        [JsonProperty("Login")]
        public string Login { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("Rating")]
        public int Rating { get; set; }

        public Person(string login, string password, int rating)
        {
            Login = login;
            Password = password;
            Rating = rating;
        }
        public Person(string login, string password)
        {
            Login = login;
            Password = password;
        }
        public Person()
        {
            Login = "";
            Password = "";
            Rating = 0;
        }
        public Person(Person person)
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
        public static bool operator ==(Person player1, Person player2)
        {
            return player1.Login == player2.Login && player1.Password == player2.Password;
        }
        public static bool operator !=(Person player1, Person player2)
        {
            return player1.Login != player2.Login || player1.Password != player2.Password;
        }
    }
}
