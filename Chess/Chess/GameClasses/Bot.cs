using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        public Way BotTurn(Field field)
        {
            double maxEnPotentProf;
            Color enColor = Program.SwitchCol(Color);
            Side enSide = Program.SwitchSide(Side);
            List<Way> legalWays = field.LegalWays(Color, Side);
            foreach (Way way in legalWays)
            {
                CountTransformationProfit(field, way, Color, Side);
                way.Profit += CuttedOffKingDist(field, way, Color, Side);
                if (way.AttackWay) way.Profit += way.EnChessPiece.PieceValue();
                CountPotentialProfit(field, way, Color, Side);
                way.Profit -= GuardWaysProfit(field, way.Start());
                field.MovePiece(way);
                way.Profit += GuardWaysProfit(field, way.End());
                maxEnPotentProf = MaxPotentialProfit(field, enColor, enSide);
                if (maxEnPotentProf > 0) way.Profit -= maxEnPotentProf;
                way.Profit -= MaxTransformationProfit(field, enColor, enSide);
                List<Way> enAttaks = FindAttackWays(field, enColor, enSide);
                if (enAttaks.Count > 0)
                {
                    double maxEnProfit = enAttaks.Max(x => x.Profit);
                    if (maxEnProfit > 0) way.Profit -= maxEnProfit;
                }
                if (!field.KingInCheck(Color, Side))
                {
                    if (field.Checkmate(enColor, enSide))
                    {
                        field.ReverseMove(way);
                        return way;
                    }
                }
                if (way.SpecialType == SpecialWayType.Castling) way.Profit += 4;
                field.ReverseMove(way);
            }
            double maxProfit = legalWays.Max(x => x.Profit);
            List<Way> res = legalWays.Where(x => x.Profit == maxProfit).ToList();
            Way resultWay = res[new Random().Next(0, res.Count)];
            if (resultWay.GetPieceType(field) == PieceType.Pawn &&
               (resultWay.End().CoordY == 0 || resultWay.End().CoordY == field.Cells.Count - 1))
                resultWay.ChessPiece.Type = PieceType.Queen;
            return resultWay;
        }
        public void CountTransformationProfit(Field field, Way way, Color color, Side side)
        {
            if (way.ChessPiece.Type != PieceType.Pawn) return;
            Direction dir;
            if (side == Side.Top) dir = Direction.Down;
            else dir = Direction.Up;
            List<Way> ways = field.DirectedWays(Color, way.Start(), new List<Direction> { dir });
            if (field[ways.Last().End()].IsEmpty &&
               VerifySafty(way, color, side, field))
            {
                int distance;
                if (side == Side.Bottom) distance = way.End().CoordY;
                else distance = field.Cells.Count - way.End().CoordY - 1;
                way.Profit += 9.0f / (distance + 1);
            }
        }
        public double MaxTransformationProfit(Field field, Color color, Side side)
        {
            List<Way> pawnWays = field.LegalWays(color, side).Where(x => x.GetPieceType(field) == PieceType.Pawn).ToList();
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
                field.MovePiece(way);
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
        public void CountPotentialProfit(Field field, Way way, Color color, Side side)
        {
            if (VerifySafty(way, color, side, field))
            {
                field.MovePiece(way);
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
                        way.Profit += maxProfit / 3;
                    }
                }
                field.ReverseMove(way);
            }
        }
        public double MaxPotentialProfit(Field field, Color color, Side side)
        {
            List<Way> ways = field.LegalWays(color, side);
            foreach (Way way in ways)
            {
                CountPotentialProfit(field, way, color, side);
            }
            if (ways.Count != 0) return ways.Max(x => x.Profit);
            else return 0;
        }
        public bool VerifySafty(Way way, Color color, Side side, Field field)
        {
            Color enColor = Program.SwitchCol(color);
            Side enSide = Program.SwitchSide(side);
            field.MovePiece(way);
            List<Way> enAttacks = FindAttackWays(field, enColor, enSide);
            if (enAttacks.Count > 0) enAttacks = enAttacks.Where(x => x.End() == way.End()).ToList();
            field.ReverseMove(way);
            return enAttacks.Count == 0 || enAttacks.Max(x => x.Profit) < 0;
        }
        public void CountExchangeProfit(Field field, List<double> profits, Way curWay, Color color, Point point, Color curColor, Side curSide)
        {
            field.MovePiece(curWay);
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
        public List<Way> FindAttackWays(Field field, Color color, Side side)
        {
            List<Way> legalWays = field.LegalWays(color, side);
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
        public bool CellUnderAttack(Field field, Point point)
        {
            return field.LegalWaysFromPoint(point, Program.SwitchCol(Color), Program.SwitchSide(Side)).Count != 0;
        }
        public double GuardWaysProfit(Field field, Point point)
        {
            double resProfit = 0;
            Color enColor = Program.SwitchCol(Color);
            Side enSide = Program.SwitchSide(Side);
            List<Way> enAttackWays = FindAttackWays(field, enColor, enSide);
            List<Way> guardWays = field.WaysOfProtection(point, Color, Side);
            if (guardWays.Count == 0) return resProfit;
            else guardWays = guardWays.Where(x => VerifySafty(x, Color, Side, field)).ToList();
            foreach (Way attackWay in enAttackWays)
            {
                if (attackWay.Profit <= 0)
                {
                    foreach (Way guardWay in guardWays)
                    {
                        if (attackWay.End() == guardWay.End())
                        {
                            // to verify legal
                            field.MovePiece(attackWay);
                            bool legal = field.VerifyLegal(guardWay, Color, Side);
                            field.ReverseMove(attackWay);
                            if (legal)
                            {
                                bool king = guardWay.GetPieceType(field) != PieceType.King;
                                if (king)
                                {
                                    field[guardWay.Start()].IsEmpty = true;
                                }
                                List<double> profits = new List<double>();
                                CountExchangeProfit(field, profits, attackWay, enColor, attackWay.End(), enColor, enSide);
                                if (king)
                                {
                                    field[guardWay.Start()].ChessPiece = guardWay.ChessPiece;
                                    field[guardWay.Start()].IsEmpty = false;
                                }
                                if (attackWay.Profit < profits.Max()) resProfit += -attackWay.Profit;
                            }
                        }
                    }
                }
            }
            return resProfit;
        }
    }
}
