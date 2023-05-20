using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
namespace RealPW
{
    class PathFinder
    {
        public static List<short[]> map = new List<short[]>();
        public static int WorldSizeX { get; set; } = 80;
        public static int WorldSizeY { get; set; } = 60;
        public static List<short> BackBlock = new List<short>()
        {
            277,267,293,110,2504,1345,0,2766,2904,1956,1906,1535,1971,1982,329
        };
        public static void ConvertByteToMap(byte[] BlockLayer)
        {
            List<short[]> map = new List<short[]>();
            byte[] ReverseBlockLayer = new byte[BlockLayer.Length];
            Array.Copy(BlockLayer, 0, ReverseBlockLayer, 0, BlockLayer.Length);
            Array.Reverse(ReverseBlockLayer);
            for (int i = 0; i < BlockLayer.Length; i = i + 160)
            {
                short[] X = new short[80];
                for (int j = 0; j < 160; j = j + 2)
                {

                    X[j / 2] = BitConverter.ToInt16(BlockLayer, i + j);
                    if (j / 2 == 79)
                    {
                        map.Insert(0, X);
                    }
                }
            }
            map.Reverse();
            PathFinder.map = map;
        }
        public static List<Point> FindPath(byte[] BlockLayer, int StartX, int StartY, int FinishX, int FinishY)
        {
            List<Point> Path = new List<Point>();
            List<short[]> map = new List<short[]>();
            byte[] ReverseBlockLayer = new byte[BlockLayer.Length];
            Array.Copy(BlockLayer, 0, ReverseBlockLayer, 0, BlockLayer.Length);
            Array.Reverse(ReverseBlockLayer);
            /*Console.WriteLine("Reverse Block Layer :");
            Console.WriteLine(BitConverter.ToString(ReverseBlockLayer));
            Console.WriteLine("Block Layer :");
            Console.WriteLine(BitConverter.ToString(BlockLayer));
            Console.WriteLine("Block Layer Shorts : ");*/
            for (int i = 0; i < BlockLayer.Length; i = i + 160)
            {
                short[] X = new short[80];
                for (int j = 0; j < 160; j = j + 2)
                {

                    X[j / 2] = BitConverter.ToInt16(BlockLayer, i + j);
                    if (j / 2 == 79)
                    {
                        map.Insert(0, X);
                    }
                }
            }

            /*Console.WriteLine("Starting Map looks like :");
            foreach (var xx in map)
            {
                foreach (var yy in xx)
                {
                    //Console.Write(yy);
                }
                Console.WriteLine();
            }*/
            map.Reverse();
            //List<short[]> mapreverse = map;
            /*foreach(var item in GetAllXBlocks(map,CurrentPosition.Y))
            {
                Console.WriteLine(item);
            }*/
            //Console.WriteLine(mapreverse[30][40]);
            //Console.WriteLine(GetBlock(mapreverse,30,40));
            var start = new Tile();
            start.Y = StartY;
            start.X = StartX;


            var finish = new Tile();
            finish.Y = FinishY;
            finish.X = FinishX;

            start.SetDistance(finish.X, finish.Y);

            var activeTiles = new List<Tile>();
            activeTiles.Add(start);
            var visitedTiles = new List<Tile>();

            while (activeTiles.Any())
            {
                var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();

                if (checkTile.X == finish.X && checkTile.Y == finish.Y)
                {
                    //We found the destination and we can be sure (Because the the OrderBy above)
                    //That it's the most low cost option. 
                    var tile = checkTile;
                    Console.WriteLine("Retracing steps backwards...");
                    while (true)
                    {
                        Path.Insert(0,new Point(tile.X, tile.Y));
                        Console.WriteLine($"{tile.X} : {tile.Y}");

                        /*if (BackBlock.Contains(map[tile.Y][tile.X]))
                        {
                            var newMapRow = map[tile.Y];
                            newMapRow[tile.X] = 0x9;
                            map[tile.Y] = (newMapRow);
                            Array.Copy(newMapRow, 0, map[tile.Y],0,newMapRow.Length-1);
                        }*/
                        tile = tile.Parent;
                        if (tile == null)
                        {
                            /*Console.WriteLine("Path looks like :");
                            foreach(var xx in map)
                            {
                                foreach(var yy in xx)
                                {
                                    Console.Write(yy);
                                }
                                Console.WriteLine();
                            }*/
                            Console.WriteLine("Done!");
                            return Path;
                        }
                    }
                }

                visitedTiles.Add(checkTile);
                activeTiles.Remove(checkTile);

                var walkableTiles = GetWalkableTiles(map, checkTile, finish);

                foreach (var walkableTile in walkableTiles)
                {
                    //We have already visited this tile so we don't need to do so again!
                    if (visitedTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                        continue;

                    //It's already in the active list, but that's OK, maybe this new tile has a better value (e.g. We might zigzag earlier but this is now straighter). 
                    if (activeTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                    {
                        var existingTile = activeTiles.First(x => x.X == walkableTile.X && x.Y == walkableTile.Y);
                        if (existingTile.CostDistance > checkTile.CostDistance)
                        {
                            activeTiles.Remove(existingTile);
                            activeTiles.Add(walkableTile);
                        }
                    }
                    else
                    {
                        //We've never seen this tile before so add it to the list. 
                        activeTiles.Add(walkableTile);
                    }
                }
            }
            Console.WriteLine("No Path Found!");
            return new List<Point>();
        }
        public static short GetBlock(int x, int y)
        {
            return map[y][x];
        }
        public static short[] GetAllXBlocks(int y)
        {
            return map[y];
        }
        public static short[] GetAllYBlocks(int X)
        {
            int ii = 0;
            short[] YBlocks = new short[map.Count];
            foreach (short[] item in map)
            {
                YBlocks[ii] = item[X];
                ii++;
            }
            return YBlocks;
        }
        private static List<Tile> GetWalkableTiles(List<short[]> map, Tile currentTile, Tile targetTile)
        {
            var possibleTiles = new List<Tile>()
            {
                new Tile { X = currentTile.X, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new Tile { X = currentTile.X, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new Tile { X = currentTile.X - 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new Tile { X = currentTile.X + 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
            };

            possibleTiles.ForEach(tile => tile.SetDistance(targetTile.X, targetTile.Y));

            var maxX = map.First().Length - 1;
            var maxY = map.Count - 1;

            return possibleTiles.Where(tile => tile.X >= 0 && tile.X <= maxX).Where(tile => tile.Y >= 0 && tile.Y <= maxY).Where(tile => BackBlock.Contains(map[tile.Y][tile.X]))
                .ToList();
        }
    }
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point(int x, int y) => (X, Y) = (x, y);
        public static Vector2 ToVector(Point point)
        {
            return new Vector2(point.X,point.Y);
        }
        public static Point ToPoint(Vector2 vec)
        {
            return new Point((int)Math.Round(vec.X,0), (int)Math.Round(vec.Y, 0));
        }
    }
    class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Cost { get; set; }
        public int Distance { get; set; }
        public int CostDistance => Cost + Distance;
        public Tile Parent { get; set; }

        //The distance is essentially the estimated distance, ignoring walls to our target. 
        //So how many tiles left and right, up and down, ignoring walls, to get there. 
        public void SetDistance(int targetX, int targetY)
        {
            this.Distance = Math.Abs(targetX - X) + Math.Abs(targetY - Y);
        }
    }
}
