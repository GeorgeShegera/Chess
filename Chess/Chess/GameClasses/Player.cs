using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chess
{
    [Serializable]
    public class Player
    {
        [JsonProperty("Login")]
        public string Login { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("Color")]
        public Color Color { get; set; }

        [JsonProperty("Side")]
        public Side Side { get; set; }

        [JsonProperty("Rating")]
        public int Rating { get; set; }

        public Player(string login, string password, Color color, Side side, int rating)
        {
            Login = login;
            Password = password;
            Color = color;
            Side = side;
            Rating = rating;
        }
    }
}
