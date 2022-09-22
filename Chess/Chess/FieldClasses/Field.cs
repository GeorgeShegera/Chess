using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
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
        public List<List<Cell>> Cells { get; set; }

        public Field(List<List<Cell>> cells)
        {
            Cells = cells;
        }
        public Field()
        {
            Cells = new List<List<Cell>>();
        }
        public Color SwitchColor(Color color)
        {
            if (color == Color.White)
            {
                return Color.Black;
            }
            else
            {
                return Color.White;
            }
        }

        public void Fill(int length, int height, Color color)
        {
            Color curCellCol = Color.White;
            for (int i = 0; i < height; i++)
            {
                Cells.Add(new List<Cell>());
                for (int j = 0; j < length; j++)
                {
                    TypeOfFigure curType = new TypeOfFigure();
                    bool isEmpty = true;
                    if (i <= 1 || i >= height - 2)
                    {
                        isEmpty = false;
                        if (i == 1 || i == height - 2)
                        {
                            curType = TypeOfFigure.Pawn;
                        }
                        else if (j == 0 || j == length - 1)
                        {
                            curType = TypeOfFigure.Rook;
                        }
                        else if (j == 1 || j == length - 2)
                        {
                            curType = TypeOfFigure.Knight;
                        }
                        else if (j == 2 || j == length - 3)
                        {
                            curType = TypeOfFigure.Bishop;
                        }
                        else if (j == length / 2)
                        {
                            curType = TypeOfFigure.King;
                        }
                        else
                        {
                            curType = TypeOfFigure.Queen;
                        }
                    }
                    Figure figure = new Figure(color, curType);
                    Cells[i].Add(new Cell(curCellCol, figure, isEmpty, false));
                    curCellCol = Program.SwitchColor(curCellCol);
                }
                curCellCol = Program.SwitchColor(curCellCol);
                if(i == height / 2)
                {
                    color = Program.SwitchColor(color);
                }
            }
        }
        public void Show()
        {
            Console.WriteLine("    A  B  C  D  E  F  G  H  ");
            Console.WriteLine("  --------------------------");
            for (int i = 0; i < Cells.Count; i++)
            {
                Console.Write(Cells.Count - i + " |");
                for (int j = 0; j < Cells[i].Count; j++)
                {
                    if (Cells[i][j].Color == Color.White)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                    }
                    if (!Cells[i][j].IsEmpty)
                    {
                        if (Cells[i][j].Figure.Color == Color.White)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                        }

                        TypeOfFigure type = Cells[i][j].Figure.Type;
                        if (type == TypeOfFigure.Pawn)
                        {
                            Console.Write(" P ");
                        }
                        else if (type == TypeOfFigure.Rook)
                        {
                            Console.Write(" R ");
                        }
                        else if (type == TypeOfFigure.Knight)
                        {
                            Console.Write(" N ");
                        }
                        else if (type == TypeOfFigure.Bishop)
                        {
                            Console.Write(" B ");
                        }
                        else if (type == TypeOfFigure.King)
                        {
                            Console.Write(" K ");
                        }
                        else
                        {
                            Console.Write(" Q ");
                        }
                    }
                    else
                    {
                        Console.Write("   ");
                    }
                    Console.ResetColor();
                }
                Console.WriteLine("|");
            }
            Console.WriteLine("  --------------------------");
        }
    }
}
