using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chess
{
    [Serializable]
    public class Player : User
    {
        [JsonProperty("Color")]
        public Color Color { get; set; }

        [JsonProperty("Side")]
        public Side Side { get; set; }

        [JsonProperty("Winner")]
        public bool Winner { get; set; }

        public Player(string login, string password, Color color, Side side, int rating, bool winner) 
            : base(login, password, rating)
        {            
            Color = color;
            Side = side;
            Rating = rating;
            Winner = winner;
        }
        public Player(Player player)
            : base(player.Login, player.Password, player.Rating)
        {
            Color = player.Color;
            Side = player.Side;
            Rating = player.Rating;
            Winner = player.Winner;
        }

        public Player(User person)
            : base(person)
        {
            Color = Color.White;
            Side = Side.Bottom;            
            Winner = false;
        }

        public Player()
            : base()
        {            
            Color = Color.White;
            Side = Side.Bottom;            
            Winner = false;
        }

        public Player(int rating, string login)
        {
            Rating = rating;
            Login = login;
            Password = "";
            Color = new Color();
            Side = new Side();
            Winner = false;
        }
        public void SwitchColor()
        {
            Color = Program.SwitchCol(Color);
        }
        public void SwitchSide()
        {
            Side = Program.SwitchSide(Side);
        }
    }
}
