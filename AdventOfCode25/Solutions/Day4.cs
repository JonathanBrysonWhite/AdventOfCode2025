using AdventOfCode25.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode25.Solutions
{
    internal class Direction
    {
        HashSet<(int, int)> Values = new HashSet<(int, int)>
        {
            //dy, dx
            (-1, 0),
            (-1, 1),
            (0, 1),
            (1, 1),
            (1, 0),
            (1, -1),
            (0, -1),
            (-1, -1)
        };

        public HashSet<(int, int)> GetValidDirections(int i, int j, int rows, int cols)
        {
            HashSet<(int, int)> ValidDirections = new();
            foreach(var (dy, dx) in Values)
            {
                int y = i + dy, x = j + dx;
                if(y >= 0 && y < rows && x >= 0 && x < cols)
                {
                    ValidDirections.Add((y, x));
                }
            }
            return ValidDirections;
        }
    }

    internal static class Day4
    {
        public static void Solve()
        {
            int answer = 0;
            Direction directions = new Direction();
            Input input = Input.FromFile("Inputs/Day4.txt");
            string[] grid = input.Lines;
            int rows = grid.Length, cols = grid[0].Length;
            for(int i = 0; i < rows; i++)
            {
                string line = "";
                for(int j = 0; j < cols; j++)
                {
                    int adjacentRolls = 0;
                    if (grid[i][j] != '@')
                    {
                        line += ".\t";
                        continue;
                    }
                        
                    foreach(var (y, x) in directions.GetValidDirections(i, j, rows, cols))
                    {
                        if (grid[y][x] == '@')
                        {
                            adjacentRolls++;
                        }
                    }
                    if (adjacentRolls < 4)
                    {
                        line += "x\t";
                        answer++;
                    }
                    else
                    {
                        line += "@\t";
                    }
                }
                Console.WriteLine(line);
            }
            Console.WriteLine(answer);
        }

        public static void Solve2()
        {
            int answer = 0;
            Direction directions = new Direction();
            Input input = Input.FromFile("Inputs/Day4.txt");
            string[] grid = input.Lines;
            int rows = grid.Length, cols = grid[0].Length;
            bool rollsAccessible = true;
            while(rollsAccessible)
            {
                List<(int, int)> rollsToRemove = new List<(int, int)>();

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        int adjacentRolls = 0;
                        if (grid[i][j] != '@')
                        {
                            continue;
                        }
                        foreach( var (y, x) in directions.GetValidDirections(i, j, rows, cols))
                        {
                            if (grid[y][x] == '@')
                            {
                                adjacentRolls++;
                            }
                        }
                        if(adjacentRolls < 4)
                        {
                            rollsToRemove.Add((i, j));
                        }
                    }
                }
                if(rollsToRemove.Count > 0)
                {
                    foreach( var (y, x) in rollsToRemove)
                    {
                        answer++;
                        string s = grid[y];
                        char[] array = s.ToCharArray();
                        array[x] = '.';
                        grid[y] = new string(array);
                    }
                }
                else
                {
                    rollsAccessible = false;
                }
            }
            Console.WriteLine(answer);
        }
    }
}
