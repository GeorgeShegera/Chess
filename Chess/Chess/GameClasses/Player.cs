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

        public Player(string login, string password)
        {
            Login = login;
            Password = password;
            Color = new Color();
            Side = new Side();
            Rating = 0;
        }

        public Player()
        {
            Login = "";
            Password = "";
            Color = Color.White;
            Side = Side.Bottom;
            Rating = 0;
        }

        public Player(int rating, string login)
        {
            Rating = rating;
            Login = login;
            Password = "";
            Color = new Color();
            Side = new Side();
        }
        public void SwitchColor()
        {
            Color = Program.SwitchColor(Color);
        }
        public void SwitchSide()
        {
            Side = Program.SwitchSide(Side);
        }
        public bool Authorized()
        {
            return Login != "" && Password != "";
        }
        public static bool operator ==(Player player1, Player player2)
        {
            return player1.Login == player2.Login && player1.Password == player2.Password;
        }
        public static bool operator !=(Player player1, Player player2)
        {
            return player1.Login != player2.Login || player1.Password != player2.Password;
        }
    }
}
