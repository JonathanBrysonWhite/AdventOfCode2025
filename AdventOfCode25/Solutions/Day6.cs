using AdventOfCode25.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdventOfCode25.Solutions
{

    public enum Operation
    {
        Add,
        Multiply
    }


    internal static class Day6
    {
        public record Problem(IList<int> nums, Operation operation);

        public static List<Problem> Problems(this Input input)
        {
            List<Problem> problems = new();
            int m = input.Lines.Length;
            int n = input.Lines[0].Length;
            List<List<int>> allNums = new();

            for (int i = 0; i < m - 1; i++)
            {
                List<int> line = input.Lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(s => int.Parse(s)).ToList();
                allNums.Add(line);
            }
            List<Operation> operations = input.Lines[m - 1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(s => s.ToOperation()).ToList();

            for(int i = 0; i < operations.Count; i++)
            {
                List<int> nums = new();
                for(int j = 0; j < allNums.Count; j++)
                {
                    nums.Add(allNums[j][i]);
                }
                problems.Add(new Problem(nums, operations[i]));
            }

            return problems;
        }

        public static List<Problem> CephalopodProblems(this Input input)
        {
            int m = input.Lines.Length;
            int n = input.Lines[0].Length;

            List<Problem> problems = new();
            List<int> nums = new();
            for (int j = n - 1; j >= 0; j--)
            {
                StringBuilder numStr = new();
                for(int i = 0; i < m; i++)
                {
                    char c = input.Lines[i][j];
                    if (c == ' ')
                    {
                        continue;
                    }
                    else if (c == '*' || c == '+')
                    {
                        nums.Add(int.Parse(numStr.ToString()));
                        problems.Add(new Problem(nums, c.ToString().ToOperation()));
                        nums = new();
                        numStr.Clear();
                    }
                    else
                    {
                        numStr.Append(c);
                    }
                }
                if(numStr.ToString().Length > 0)
                    nums.Add(int.Parse(numStr.ToString()));
                numStr.Clear();
            }
            return problems;
        }

        public static void Solve2()
        {
            long answer = 0;
            List<Problem> problems = Input.FromFile("Inputs/Day6.txt", StringSplitOptions.None).CephalopodProblems();
            foreach(Problem problem in problems)
            {
                long result = GetIdentityValue(problem.operation);
                foreach(int num in problem.nums)
                {
                    result = Eval(result, num, problem.operation);
                }
                answer += result;
            }
            Console.WriteLine(answer);
        }

        public static void Solve()
        {
            long answer = 0;
            List<Problem> problems = Input.FromFile("Inputs/Day6.txt").Problems();
            foreach(Problem problem in problems)
            {
                Console.WriteLine($"answer: {answer}");
                Console.WriteLine($"Evaluating Problem: {JsonSerializer.Serialize(problem)}");
                long res = GetIdentityValue(problem.operation);
                foreach(int num in problem.nums)
                {
                    res = Eval(res, num, problem.operation);
                    //Console.WriteLine($"result: {res}");
                }
                answer += res;
            }
            Console.WriteLine(answer);
        }
        public static int GetIdentityValue(Operation operation)
        {
            switch (operation)
            {
                case Operation.Add:
                    return 0;
                case Operation.Multiply:
                    return 1;
                default: return 0;
            }
        }
        public static long Eval(long x, int y, Operation operation)
        {
            switch (operation)
            {
                case Operation.Add:
                    return x + y;
                case Operation.Multiply:
                    return x * y;
                default: return 0;
            }
        }
    }
}
