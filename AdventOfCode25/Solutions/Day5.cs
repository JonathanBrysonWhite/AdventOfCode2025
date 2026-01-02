using AdventOfCode25.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode25.Solutions
{
    internal static class Day5
    {

        public static ((long, long)[], long[]) Ingredients(this Input input)
        {
            (long, long)[] ranges = input.Groups()[0].Select(l =>
            {
                long[] ints = l.Split('-').Select(n => long.Parse(n)).ToArray();
                return (ints[0], ints[1]);
            }).ToArray();
            long[] ingredients = input.Groups()[1].Select(l => long.Parse(l)).ToArray();
            return (ranges, ingredients);
        }

        public static void Solve()
        {
            int answer = 0;
            Input input = Input.FromFile("Inputs/Day5.txt");
            ((long, long)[] ranges, long[] ingredients) = input.Ingredients();
            foreach(long ingredient in ingredients)
            {
                foreach((long low, long high) in ranges)
                {
                    if(ingredient >= low && ingredient <= high)
                    {
                        answer++;
                        break;
                    }
                }
            }

            Console.WriteLine(answer);
        }

        public static void Solve2()
        {
            long answer = 0;
            Input input = Input.FromFile("Inputs/Day5.txt");
            ((long, long)[] ranges, _) = input.Ingredients();
            List<(long, long)> actualRanges = new();
            Array.Sort(ranges, (a, b) => {
                int cmp = a.Item1.CompareTo(b.Item1);
                return cmp != 0 ? cmp : a.Item2.CompareTo(b.Item2);
            });
            long lastStart = ranges[0].Item1;
            long lastEnd = ranges[0].Item2;

            for(int i = 0; i < ranges.Length - 1; i++)
            {
                if (ranges[i].Item2 <= ranges[i + 1].Item1 - 1)
                {
                    actualRanges.Add((lastStart, ranges[i].Item2 > lastEnd ? ranges[i].Item2 : lastEnd));
                    lastStart = ranges[i + 1].Item1;
                    lastEnd = ranges[i + 1].Item2;
                }
            }
            actualRanges.Add((lastStart, ranges[ranges.Length - 1].Item2 > lastEnd ? ranges[ranges.Length - 1].Item2 : lastEnd));

            foreach((long start, long end) in actualRanges)
            {
                answer += (end - start + 1);
            }
            Console.WriteLine(answer);
        }
    }
}
