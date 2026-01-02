using AdventOfCode25.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace AdventOfCode25.Solutions
{
    public class Point
    { 
        public int X { get; }
        public int Y { get; }
        public int Z { get; }
        public Point(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double Distance(Point other)
        {
            return Math.Sqrt(Math.Pow(other.X - X, 2) + Math.Pow(other.Y - Y, 2) + Math.Pow(other.Z - Z, 2));
        }

        public double Magnitude()
        {
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
        }
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is Point)
            {
                Point? other = obj as Point;
                if (other == null) return false;
                return other.X == X && other.Y == Y && other.Z == Z;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
    }

    public class Connection
    {
        public Point A { get; set; }
        public Point B { get; set; }
        public double Distance { get; set; }

        public Connection(Point a, Point b)
        {
            A = a;
            B = b;
            Distance = A.Distance(B);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if(obj is Connection c)
            {
                return (A.Equals(c.A) && B.Equals(c.B)) || (A.Equals(c.B) && B.Equals(c.A));
            }
            return false;
        }

        public override int GetHashCode()
        {
            if(A.Magnitude() <= B.Magnitude())
            {
                return HashCode.Combine(A, B);
            }
            else
            {
                return HashCode.Combine(B, A);
            }
        }
    }


    public sealed class PointUnionFind
    {
        private readonly Dictionary<Point, Point> parent = new();
        private readonly Dictionary<Point, int> rank = new();
        private readonly Dictionary<Point, HashSet<Point>> components = new();
        private readonly Dictionary<Point, int> sizes = new();
        public int Count { get; private set; }

        public PointUnionFind(List<Point> points)
        {
            foreach(Point p in points)
            {
                Add(p);
            }
        }

        public void Add(Point p)
        {
            if (parent.ContainsKey(p))
                return;
            parent[p] = p;
            rank[p] = 0;
            sizes[p] = 1;
            components[p] = new HashSet<Point> { p };
            Count++;
        }

        public Point Find(Point p)
        {
            if (!parent.TryGetValue(p, out var root))
                throw new InvalidDataException();
            if (!root.Equals(p))
            {
                parent[p] = Find(root);
            }

            return parent[p];
        }

        public void Union(Point a, Point b)
        {
            Add(a);
            Add(b);

            var rootA = Find(a);
            var rootB = Find(b);

            if(rootA.Equals(rootB))
            {
                return;
            }

            if (rank[rootA] < rank[rootB])
            {
                (rootA, rootB) = (rootB, rootA);
            }

            parent[rootB] = rootA;
            sizes[rootA] += sizes[rootB];

            if (rank[rootA] == rank[rootB])
            {
                rank[rootA]++;
            }
            Count--;
        }

        public long GetCombinedTopSizes(int n)
        {
            return sizes.Where(s => parent[s.Key] == s.Key).OrderByDescending(s => s.Value).Take(n).ToList().Aggregate(1L, (acc, val) => acc * val.Value);
        }
    }


    internal static class Day8
    {
        internal static List<Point> Points(this Input input)
        {
            return input.Lines
                .Select(l => 
                    l.Split(',')
                    .Select(p => 
                        int.Parse(p))
                    .ToArray())
                .Select(i => 
                { 
                    return new Point(i[0], i[1], i[2]); 
                })
                .ToList();
        }

        public static void Solve2()
        {
            List<Point> points = Input.FromFile("Inputs/Day8.txt").Points();
            PointUnionFind circuits = new PointUnionFind(points);
            PriorityQueue<Connection, double> minHeap = new PriorityQueue<Connection, double>();
            HashSet<Connection> visited = new HashSet<Connection>();
            foreach(Point p in points)
            {
                foreach(Point other in points)
                {
                    if(p.Equals(other))
                    {
                        continue;
                    }
                    Connection conn = new Connection(p, other);
                    if(visited.Add(conn))
                    {
                        minHeap.Enqueue(conn, conn.Distance);
                    }
                }
            }
            Connection last = null;
            while(circuits.Count > 1 )
            {
                Connection c = minHeap.Dequeue();
                if(c.A.Equals(c.B))
                {
                    continue;
                }
                last = c;
                circuits.Union(last.A, last.B);
            }
            long ans = (long)last.A.X * (long)last.B.X;
            Console.WriteLine($"{ans}");
        }
        public static void Solve()
        {
            const int NUM_LARGEST_CIRCUITS = 3;
            const int NUM_SHORTEST_CONNECTIONS = 1000; 
            List<Point> points = Input.FromFile("Inputs/Day8.txt").Points();
            PointUnionFind circuits = new PointUnionFind(points);
            PriorityQueue<Connection, double> minHeap = new PriorityQueue<Connection, double>();
            HashSet<Connection> visited = new HashSet<Connection>();

            foreach(Point p in points)
            {
                foreach(Point other in points)
                {
                    if(p.Equals(other))
                    {
                        continue;
                    }
                    Connection conn = new Connection(p, other);
                    if(visited.Add(conn))
                    {
                        minHeap.Enqueue(conn, conn.Distance);
                        if(conn.Distance < 0)
                        {
                            Console.WriteLine(conn.Distance);

                        }
                    }
                }
            }
            //dequeue item
            //check if two points are in circuit -> if yes, do nothing
            //combine circuits for the two points
            for(int i = 0; i < NUM_SHORTEST_CONNECTIONS; i++)
            {
                Connection c = minHeap.Dequeue();
                if(circuits.Find(c.A).Equals(circuits.Find(c.B)))
                {
                    continue;
                }
                else
                {
                    circuits.Union(c.A, c.B);
                }
            }

            Console.WriteLine(circuits.GetCombinedTopSizes(NUM_LARGEST_CIRCUITS));
        }
    }
}
