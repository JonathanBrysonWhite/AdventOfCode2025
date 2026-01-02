using AdventOfCode25.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode25.Solutions
{
    internal class Day7
    {

        public static void Solve()
        {
            int answer = 0;
            string[] lines = Input.FromFile("Inputs/Day7.txt").Lines;
            int m = lines.Length, n = lines[0].Length;
            HashSet<int> tachyonIndices = new HashSet<int>
            {
                lines[0].IndexOf('S')
            };
            for(int i = 1; i < m; i++)
            {
                List<int> splitterIndices = new();
                int splitterIndex = 0;
                while((splitterIndex = lines[i].IndexOf('^', splitterIndex)) != -1)
                {
                    splitterIndices.Add(splitterIndex++);
                }
                List<int> hits = splitterIndices.Where(x => tachyonIndices.Contains(x)).ToList();
                answer += hits.Count;
                foreach (int hit in hits)
                {
                    tachyonIndices.Remove(hit);
                    if(hit > 0 && !tachyonIndices.Contains(hit - 1))
                    {
                        tachyonIndices.Add(hit - 1);
                    }
                    if(hit < n - 1 && !tachyonIndices.Contains(hit + 1))
                    {
                        tachyonIndices.Add(hit + 1);
                    }
                }
            }
            Console.WriteLine(answer);
        }

        public static void Solve2()
        {
            string[] lines = Input.FromFile("Inputs/Day7.txt").Lines;
            int m = lines.Length, n = lines[0].Length;
            long[][] dp = new long[m][];
            for(int i = 0; i < m; i++)
            {
                dp[i] = new long[n];
            }
            int indexOfOrigin = lines[0].IndexOf('S');
            dp[0][indexOfOrigin] = 1;
            for(int i = 1; i < m; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    long prevBeam = dp[i - 1][j];
                    if (lines[i][j] == '^')
                    {
                        if(j > 0)
                        {
                            dp[i][j - 1] += prevBeam;
                        }
                        if(j < n - 1)
                        {
                            dp[i][j + 1] += prevBeam;
                        }
                    }
                    else
                    {
                        dp[i][j] += prevBeam;
                    }
                }
            }
            long answer = 0;
            for(int i = 0; i < n; i++)
            {
                answer += dp[m - 1][i];
            }
            Console.WriteLine(answer);
        }
    }
}
