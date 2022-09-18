using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Chess
{
    [Serializable]
    public class Field
    {
        [JsonProperty("Cells")]
        public List<Cell> Cells { get; set; }

        public Field(List<Cell> cells)
        {
            Cells = cells;
        }
    }
}
