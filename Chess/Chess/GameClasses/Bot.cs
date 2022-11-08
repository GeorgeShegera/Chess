using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
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
            Color = Program.SwitchCol(Color);
        }
        public void SwitchSide()
        {
            Side = Program.SwitchSide(Side);
        }

        public double CountWaysProfit(List<Way> ways)
        {
            double profit = 0f;
            foreach(Way way in ways)
            {
                if(way.Profit > 0) profit += way.Profit;
            }
            return profit;
        }
        public List<Way> SortSafetyWays(List<Way> ways, Color color, Side side, Field field)
        {
            ways = ways.Where(x => VerifySafety(x, color, side, field)).ToList();
            if (ways.Count > 0) return ways;
            else return new List<Way>();
        }
        public Way BotTurn(Field field)
        {
            //Console.Clear();
            //field.Show();
            //Console.WriteLine("Bot is thinking...");
            double bestAttackProfit;
            Color enColor = Program.SwitchCol(Color);
            Side enSide = Program.SwitchSide(Side);
            int piecesNumber = field.CountPieces();// to count number of pieces
            List<Way> legalWays = field.LegalWays(Color, Side);
            List<Way> enLegalWays = field.LegalWays(enColor, enSide);
            List<Way> enAttacksBefore = FindAttackWays(field, enColor, enSide, enLegalWays);
            double attackProfitBef = CountWaysProfit(enAttacksBefore);
            double potentProfitBef = PotentialWaysProfit(field, enColor, enSide, enLegalWays);            
            foreach (Way way in legalWays)
            {
                way.Safety = VerifySafety(way, Color, Side, field);
                CountTransformationProfit(field, way, Color, Side);// to count pawn's transformation
                if (piecesNumber < 8) way.Profit += CuttedOffKingDist(field, way, Color, Side);// to cut of enemy's ways (late game)
                if (way.AttackWay) way.Profit += way.EnChessPiece.PieceValue();// to count an attack profit
                //way.Profit -= GuardWaysProfit(field, way.Start(), Color, Side, enAttacksBefore);// to count guard prof before the move
                field.MakeMove(way);// to make the move

                List<Way> enLegWaysAfter = field.LegalWays(enColor, enSide);// to find enemy's legal ways
                List<Way> enAttaks = FindAttackWays(field, enColor, enSide, enLegWaysAfter);// to find enemy's attaks
                double enPotentProf = PotentialWaysProfit(field, enColor, enSide, enLegalWays);// to find enemy's potential ways
                List<Way> myAttaks = FindAttackWays(field, Color, Side, field.LegalWays(Color, Side));// to find enemy's attaks
                way.Profit -= CountWaysProfit(enAttaks) - attackProfitBef; // to count an attack's difference
                way.Profit -= enPotentProf - potentProfitBef;// to count potential difference
                myAttaks = SortSafetyWays(myAttaks, Color, Side, field);
                if (myAttaks.Count > 0)// to check attack count 
                {
                    bestAttackProfit = myAttaks.Max(x => x.Profit);
                    if (bestAttackProfit > 0) way.Profit += bestAttackProfit / 2;
                }



                //if (enAttaks.Count > 0)// to check attack count 
                //{
                //    bestAttackProfit = enAttaks.Max(x => x.Profit);
                //    if (way.AttackWay &&
                //        enAttaks.Any(x => x.End() == way.End() && x.Profit == bestAttackProfit))
                //    {
                //        way.Profit -= way.EnChessPiece.PieceValue();
                //    }
                //    if (bestAttackProfit > 0) way.Profit -= bestAttackProfit;
                //}

                //way.Profit += GuardWaysProfit(field, way.End(), Color, Side, enAttaks);// to count guard prof after the move
                //maxPotentialProf = MaxPotentialProfit(field, Color, Side, field.LegalWays(Color, Side));// to count my potential attack profit
                //if (maxPotentialProf > 0) way.Profit += maxPotentialProf;


                //maxPotentialProf = MaxPotentialProfit(field, enColor, enSide, enLegWaysAfter);// to count enemy's potential profit
                //if (maxPotentialProf > 0) way.Profit -= maxPotentialProf;// to verify and count enemy's potential profit
                way.Profit -= MaxTransformationProfit(field, enColor, enSide, enLegWaysAfter);// to count enemy's transformation profit
                if (field.Checkmate(enColor, enSide, enLegWaysAfter))// to verify checkmate
                {
                    field.ReverseMove(way);
                    return way;
                }
                if (way.SpecialType == SpecialWayType.Castling) way.Profit += 2;// to verify castling
                field.ReverseMove(way);
            }
            double maxProfit = legalWays.Max(x => x.Profit);
            List<Way> res = legalWays.Where(x => x.Profit == maxProfit).ToList();
            Way resultWay = res[new Random().Next(0, res.Count)];
            field.PawnTransformation(resultWay);
            return resultWay;
        }
        public void CountTransformationProfit(Field field, Way way, Color color, Side side)
        {
            if (way.ChessPiece.Type != PieceType.Pawn) return;
            Direction dir;
            if (side == Side.Top) dir = Direction.Down;
            else dir = Direction.Up;
            List<Way> ways = new List<Way> { way };
            ways.AddRange(field.DirectedWays(Color, way.End(), new List<Direction> { dir }, false));
            if (ways.Count > 0 && (ways.Last().End().CoordY == 0 ||
                ways.Last().End().CoordY == field.Cells.Count - 1) && way.Safety)
            {
                int distance;
                if (side == Side.Bottom) distance = way.End().CoordY;
                else distance = field.Cells.Count - way.End().CoordY - 1;
                way.Profit += 9.0f / (distance + 1);
            }
        }
        public double MaxTransformationProfit(Field field, Color color, Side side, List<Way> legalWays)
        {
            List<Way> pawnWays = legalWays.Where(x => x.GetPieceType(field) == PieceType.Pawn).ToList();
            if (pawnWays.Count == 0) return 0;
            foreach (Way pawnWay in pawnWays)
            {
                CountTransformationProfit(field, pawnWay, color, side);
            }
            return pawnWays.Max(x => x.Profit);
        }
        public double CuttedOffKingDist(Field field, Way way, Color color, Side side)
        {
            Color enColor = Program.SwitchCol(color);
            Side enSide = Program.SwitchSide(side);
            double distBefore;
            double distAfter;
            Point enKingPoint = field.KingPoint(enColor);
            Point myKingPoint = field.KingPoint(color);
            if (way.ChessPiece.Type == PieceType.King)
            {
                distBefore = Field.DistBetPoints(way.End(), enKingPoint);
                distAfter = Field.DistBetPoints(way.Start(), enKingPoint);
                return distAfter - distBefore;
            }
            else
            {
                distBefore = Field.DistToCentre(enKingPoint);
                field.MakeMove(way);
                List<Way> enKingWays = field.LegalWaysFromPoint(enKingPoint, enColor, enSide);
                field.ReverseMove(way);
                if (enKingWays.Count > 0)
                {
                    distAfter = enKingWays.Select(x => Field.DistToCentre(x.End())).Min();
                }
                else return 0;
            }
            return distAfter - distBefore;
        }
        public double CountPotentialProfit(Field field, Way way, Color color, Side side)
        {
            double resProfit = 0f;
            if (!way.Safety) return resProfit;
            field.MakeMove(way);
            List<Way> newWays = field.LegalWaysFromPoint(way.End(), color, side);
            foreach (Way newWay in newWays)
            {
                if (newWay.EnChessPiece.Type == PieceType.King) newWay.Profit++;
                else if (newWay.AttackWay)
                {
                    List<double> profits = new List<double>();
                    CountExchangeProfit(field, profits, newWay, color, newWay.End(), color, side);
                    newWay.Profit += profits.Max();
                }
            }
            if (newWays.Count != 0)
            {
                double maxProfit = newWays.Max(x => x.Profit);
                if (maxProfit > 0)
                {
                    resProfit += maxProfit / 3;
                }
            }
            field.ReverseMove(way);
            return resProfit;
        }
        //public double MaxPotentialProfit(Field field, Color color, Side side, List<Way> legalWays)
        //{

        //    if (legalWays.Count != 0) return legalWays.Max(x => x.Profit);
        //    else return 0;
        //}
        public double PotentialWaysProfit(Field field, Color color, Side side, List<Way> legalWays)
        {
            double resProfit = 0f;
            foreach (Way way in legalWays)
            {
                resProfit = CountPotentialProfit(field, way, color, side);
            }
            return resProfit;
        }
        public bool VerifySafety(Way way, Color color, Side side, Field field)
        {
            Color enColor = Program.SwitchCol(color);
            Side enSide = Program.SwitchSide(side);
            field.MakeMove(way);
            List<Way> enAttacks = FindAttackWays(field, enColor, enSide, field.LegalWays(enColor, enSide));
            if (enAttacks.Count > 0) enAttacks = enAttacks.Where(x => x.End() == way.End()).ToList();
            field.ReverseMove(way);
            return enAttacks.Count == 0 || enAttacks.Max(x => x.Profit) < 0;
        }
        public void CountExchangeProfit(Field field, List<double> profits, Way curWay, Color color, Point point, Color curColor, Side curSide)
        {
            field.MakeMove(curWay);
            double curProfit = curWay.EnChessPiece.PieceValue();
            if (curColor != color) curProfit *= -1;
            curColor = Program.SwitchCol(curColor);
            curSide = Program.SwitchSide(curSide);
            if (curColor != color)
            {
                if (profits.Count != 0) curProfit += profits.Last();
                profits.Add(curProfit);
            }
            else profits[profits.Count - 1] += curProfit;
            List<Way> newWays = field.LegalWaysToPoint(point, curColor, curSide);
            if (newWays.Count != 0)
            {
                Way way = newWays.OrderBy(x => x.ChessPiece.PieceValue()).ToList().First();
                CountExchangeProfit(field, profits, way, color, point, curColor, curSide);
            }
            field.ReverseMove(curWay);
        }
        public List<Way> FindAttackWays(Field field, Color color, Side side, List<Way> legalWays)
        {
            //List<Way> legalWays = field.LegalWays(color, side);
            List<Way> attackWays = new List<Way>();
            foreach (Way way in legalWays)
            {
                if (way.AttackWay && way.EnChessPiece.Type != PieceType.King)
                {
                    List<double> profits = new List<double>();
                    CountExchangeProfit(field, profits, way, color, way.End(), color, side);
                    way.Profit += profits.Max();
                    attackWays.Add(way);
                }
            }
            return attackWays;
        }
        public double GuardWaysProfit(Field field, Point point, Color color, Side side, List<Way> enAttackWays)
        {
            double resProfit = 0;
            Color enColor = Program.SwitchCol(color);
            Side enSide = Program.SwitchSide(side);
            field.Show();
            List<Way> guardWays = field.FindPieceWays(point, color, side, true);
            if (guardWays.Count == 0 || enAttackWays.Count == 0) return resProfit;
            else guardWays = guardWays.Where(x => field.VerifyLegal(x, color, side) &&
                                             VerifySafety(x, color, side, field)).ToList();
            foreach (Way attackWay in enAttackWays)
            {
                if (attackWay.Profit >= 0) continue;
                field.Show();
                foreach (Way guardWay in guardWays)
                {
                    if (attackWay.End() == guardWay.End())
                    {
                        //bool king = guardWay.GetPieceType(field) != PieceType.King;
                        field[guardWay.Start()].IsEmpty = true; // to find your chess piece 
                        List<double> profits = new List<double>(); 
                        CountExchangeProfit(field, profits, attackWay, enColor, attackWay.End(), enColor, enSide);
                        double maxProfit = profits.Max(); // to count the biggest pfrofit 
                        field[guardWay.Start()].Piece = guardWay.ChessPiece; // to return chess piece back
                        field[guardWay.Start()].IsEmpty = false; // to return chess piece back
                        if (attackWay.Profit < maxProfit) resProfit += attackWay.Profit - maxProfit;// to count the result
                    }
                }
            }
            return resProfit;
        }
    }
}
