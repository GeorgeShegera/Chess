using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chess
{
    [Serializable]
    public class Bot
    {
        [JsonProperty("Color")]
        public Color Color { get; set; }

        [JsonProperty("Side")]
        public Side Side { get; set; }

        public Bot(Color color, Side side)
        {
            Color = color;
            Side = side;
        }

        public Bot()
        {
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
    }
}
