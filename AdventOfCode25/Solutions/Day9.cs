using AdventOfCode25.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode25.Solutions
{
    public record Tile(long X, long Y);
    internal static class Day9
    {
        public static List<Tile> Tiles(this Input input)
        {
            return input.Lines.Select(l => { return new Tile(int.Parse(l.Split(',')[0]), int.Parse(l.Split(',')[1])); }).ToList();
        }
        public static void Solve()
        {
            long maxArea = 0;
            List<Tile> tiles = Input.FromFile("Inputs/Day9.txt").Tiles();
            foreach (Tile tile in tiles)
            {
                foreach (Tile otherTile in tiles)
                {
                    long area = (Math.Abs(tile.X - otherTile.X) + 1) * (Math.Abs(tile.Y - otherTile.Y) + 1);
                    if (area > maxArea)
                    {
                        maxArea = area;
                    }
                }
            }
            Console.WriteLine(maxArea);
        }

        public static void Solve2()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            List<Tile> tiles = Input.FromFile("Inputs/Day9.txt").Tiles();

            (Dictionary<long, int> xDict, Dictionary<long, int> yDict, long[] xRevDict, long[] yRevDict) = CompressCoordinates(tiles);

            int m = yDict.Count();
            int n = xDict.Count();

            bool[,] blocked = new bool[m, n];
            bool[,] inside = new bool[m, n];
            bool[,] isRed = new bool[m, n];
            int[,] prefixSum = new int[m, n];

            long answer = 0;

            foreach (Tile tile in tiles)
            {
                int x = xDict[tile.X];
                int y = yDict[tile.Y];
                isRed[y, x] = true;
            }
            Console.WriteLine($"Filled in corners: {watch.Elapsed}");

            BuildBlockedArray(tiles, xDict, yDict, blocked);

            Console.WriteLine($"Drew outline: {watch.Elapsed}");

            FloodFill(blocked, inside);

            Console.WriteLine($"Flood filled polygon: {watch.Elapsed}");

            BuildValidPrefixSum(inside, prefixSum);

            Console.WriteLine($"Built prefix sum table: {watch.Elapsed}");

            //LogArray(blocked);
            //LogArray(inside);
            //LogArray(isRed);
            //LogArray(prefixSum);

            for (int i = 0; i < tiles.Count; i++)
            {
                for(int j = i + 1; j < tiles.Count; j++)
                {
                    Tile t1 = tiles[i];
                    Tile t2 = tiles[j];

                    if(IsValidRectangle(t1, t2, xDict, yDict, prefixSum))
                    {
                        long area = (Math.Abs(t1.X - t2.X) + 1) * (Math.Abs(t1.Y - t2.Y) + 1);
                        if (area > answer)
                            answer = area;
                    }
                }
            }

            Console.WriteLine($"Iterated over all vertices: {watch.Elapsed}");
            Console.WriteLine(answer);

        }

        public static bool IsValidRectangle(Tile t1, Tile t2, Dictionary<long, int> xDict, Dictionary<long, int> yDict, int[,] prefixSum)
        {
            int t1X = xDict[t1.X],
                t1Y = yDict[t1.Y],
                t2X = xDict[t2.X],
                t2Y = yDict[t2.Y];

            int x1 = Math.Min(t1X, t2X),
                x2 = Math.Max(t1X, t2X),
                y1 = Math.Min(t1Y, t2Y),
                y2 = Math.Max(t1Y, t2Y);


            //area from prefix sum array for rectangle 
            // sum = prefix[y2, x2] - prefix[y1, x2] - prefix[y2, x1] + prefix[x1, y1]
            // where x1 < x2 and y1 < y2
            int expectedSum = prefixSum[y2, x2] - prefixSum[y1, x2] - prefixSum[y2, x1] + prefixSum[y1, x1];
            int actualSum = (x2 - x1) * (y2 - y1);
            return expectedSum == actualSum;
        }
        public static void BuildValidPrefixSum(bool[,] inside, int[,] prefixSum)
        {
            //start by pre-populating for x = 0 & y = 0
            if (inside[0,0])
            {
                prefixSum[0, 0] = 1;
            }
            for(int x = 1; x < prefixSum.GetLength(1); x++)
            {
                prefixSum[0, x] += prefixSum[0, x - 1] + (inside[0, x] ? 1 : 0);
            }
            for(int y = 1; y < prefixSum.GetLength(0); y++)
            {
                prefixSum[y, 0] += prefixSum[y - 1, 0] + (inside[y, 0] ? 1 : 0);
            }

            for(int y = 1; y < prefixSum.GetLength(0); y++)
            {
                for(int x = 1; x < prefixSum.GetLength(1); x++)
                {
                    //valid prefix sum at prefix[i,j] = arr[i,j] + prefix[i - 1, j] + prefix[i, j - 1] - prefix[i - 1, j - 1]
                    prefixSum[y, x] = (inside[y, x] ? 1 : 0) + prefixSum[y - 1, x] + prefixSum[y, x - 1] - prefixSum[y - 1, x - 1];
                }
            }
        }

        public static void FloodFill(bool[,] blocked, bool[,] inside)
        {
            int m = blocked.GetLength(0);
            int n = blocked.GetLength(1);
            bool[,] visited = new bool[m, n];
            Queue<(int x, int y)> q = new();

            for (int x = 0; x < n; x++)
            {
                q.Enqueue((x, 0));
                q.Enqueue((x, m - 1));
            }
            for (int y = 0; y < m; y++)
            {
                q.Enqueue((0, y));
                q.Enqueue((n - 1, y));
            }

            while (q.Count > 0)
            {
                (int x, int y) = q.Dequeue();
                if (x < 0 || x >= n || y < 0 || y >= m) continue;
                if (visited[y, x]) continue;
                if (blocked[y, x]) continue;

                visited[y, x] = true;

                q.Enqueue((x + 1, y));
                q.Enqueue((x - 1, y));
                q.Enqueue((x, y + 1));
                q.Enqueue((x, y - 1));
            }

            for (int y = 0; y < m; y++)
            {
                for (int x = 0; x < n; x++)
                {
                    inside[y, x] = !visited[y, x];
                }
            }

        }
        public static void BuildBlockedArray(List<Tile> tiles, Dictionary<long, int> xDict, Dictionary<long, int> yDict, bool[,] blocked)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                Tile thisTile = tiles[i];
                Tile nextTile = tiles[(i + 1) % tiles.Count];

                int x1 = xDict[thisTile.X], x2 = xDict[nextTile.X], y1 = yDict[thisTile.Y], y2 = yDict[nextTile.Y];

                bool isVertical = x1 == x2;
                if (isVertical)
                {
                    for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
                    {
                        blocked[y, x1] = true;
                    }
                }
                else
                {
                    for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
                    {
                        blocked[y1, x] = true;
                    }
                }
            }
        }
        public static (Dictionary<long, int>, Dictionary<long, int>, long[], long[]) CompressCoordinates(List<Tile> tiles)
        {
            HashSet<long> xs = [], ys = [];
            Dictionary<long, int> xDict = [], yDict = [];


            foreach (Tile tile in tiles)
            {
                xs.Add(tile.X);
                xs.Add(tile.X - 1);
                xs.Add(tile.X + 1);

                ys.Add(tile.Y);
                ys.Add(tile.Y - 1);
                ys.Add(tile.Y + 1);
            }

            long[] xList = xs.OrderBy(x => x).ToArray();
            long[] yList = ys.OrderBy(y => y).ToArray();


            for (int i = 0; i < xList.Length; i++)
            {
                xDict.Add(xList[i], i);
            }

            for (int i = 0; i < yList.Length; i++)
            {
                yDict.Add(yList[i], i);
            }

            return (xDict, yDict, xList, yList);
        }

        public static void LogArray(bool[,] array)
        {
            Console.WriteLine($"{new string('-', 25)}");
            int m = array.GetLength(0);
            int n = array.GetLength(1);

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write($"{(array[i, j] ? '#' : '.')}");
                }
                Console.WriteLine();
            }
            Console.WriteLine($"{new string('-', 25)}");
        }

        public static void LogArray(int[,] array)
        {
            Console.WriteLine($"{new string('-', 25)}");
            int m = array.GetLength(0);
            int n = array.GetLength(1);

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write($"{array[i, j]}\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine($"{new string('-', 25)}");
        }
    }
}
