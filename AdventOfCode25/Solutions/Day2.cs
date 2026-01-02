using AdventOfCode25.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode25.Solutions
{
    public static class Day2
    {
        public record Range(long start, long end);

        public static Range[] Ranges(this Input input)
        {
            return input.Raw
                .Split(',')
                .Select(i =>
                {
                    string[] ranges = i.Split('-');
                    return new Range(long.Parse(ranges[0]), long.Parse(ranges[1]));
                })
                .ToArray();
        }
        public static void Solve()
        {
            Range[] ranges = Input.FromFile("Inputs/Day2.txt").Ranges();
            long solution = 0;
            foreach(Range r in ranges)
            {
                long cur = r.start;
                do
                {
                    string curStr = cur.ToString();
                    if (curStr.Length % 2 == 0)
                    {
                        int l = curStr.Length;
                        string first = curStr.Substring(0, l / 2);
                        string second = curStr.Substring(l / 2, l / 2);
                        if (first == second)
                        {
                            solution += cur;
                        }
                    }
                    cur++;
                }
                while (cur <= r.end);

            }
            Console.WriteLine(solution);
        }

        public static void Solve2()
        {
            Range[] ranges = Input.FromFile("Inputs/Day2.txt").Ranges();
            long solution = 0;
            foreach(Range r in ranges)
            {
                long cur = r.start;
                do
                {
                    string curStr = cur.ToString();
                    if (IsRepeating(curStr))
                    {
                        solution += cur;
                    }
                    cur++;
                }
                while (cur <= r.end);
            }
            Console.WriteLine(solution);
        }

        public static bool IsRepeating(string s)
        {
            return (s + s).IndexOf(s, 1) != s.Length;
        }
    }
}
