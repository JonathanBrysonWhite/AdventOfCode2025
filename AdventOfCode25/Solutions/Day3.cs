using AdventOfCode25.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode25.Solutions
{
    internal static class Day3
    {
        private static int[][] Banks(this Input input)
        {
            int[][] banks = new int[input.Lines.Length][];
            for(int i = 0; i < banks.Length; i++)
            {
                string line = input.Lines[i];
                int[] bank = new int[line.Length];
                for(int j = 0; j < line.Length; j++)
                {
                    bank[j] = (int)Char.GetNumericValue(line[j]);
                }
                banks[i] = bank;
            }
            return banks;
        }

        public static void Solve()
        {
            int result = 0;

            int[][] banks = Input.FromFile("Inputs/Day3.txt").Banks();
            foreach(int[] bank in banks)
            {
                int max = bank.Max();
                string largestJoltage = "";
                int indexOfMax = Array.FindIndex(bank, x => x == max);
                if (indexOfMax == bank.Length - 1)
                {
                    //find second largest
                    int secondMax = (from number in bank
                                     orderby number descending
                                     select number).Skip(1).First();
                    largestJoltage += secondMax.ToString() + max.ToString();
                }
                else
                {
                    int secondMax = (from number in bank
                                     select number).Skip(indexOfMax + 1).Max();
                    largestJoltage += max.ToString() + secondMax.ToString();
                }
                result += int.Parse(largestJoltage);
            }
            Console.WriteLine(result);
        }

        public static void Solve2()
        {
            long result = 0;
            int[][] banks = Input.FromFile("Inputs/Day3.txt").Banks();
            foreach (int[] bank in banks)
            {
                int[] largest = GetLargestSubArray(bank);
                result += long.Parse(String.Join("", Array.ConvertAll<int, string>(largest, Convert.ToString)));
            }
            Console.WriteLine(result);
        }
        public static int[] GetLargestSubArray(int[] bank)
        {
            int n = bank.Length;
            //number of integers we must drop
            int numDrops = bank.Length - 12;
            int[] result = new int[12];

            int windowSize = numDrops + 1;

            int i = 0;
            int j = 0;
            while(i < n && j < 12)
            {
                int start = i;
                int max = bank[i];
                int maxIndex = i;
                for(; i < start + windowSize && i < n; i++)
                {
                    if (bank[i] > max)
                    {
                        max = bank[i];
                        maxIndex = i;
                    }
                }
                int numRemoved = maxIndex - start;
                windowSize -= numRemoved;
                result[j++] = max;
                i = maxIndex + 1;
            }
            return result;
        }
    }
}
