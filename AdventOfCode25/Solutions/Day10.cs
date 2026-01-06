using AdventOfCode25.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode25.Solutions
{
    internal static class Day10
    {
        #region PART1
        public static void Solve()
        {
            // express bit sets as matrices
            // so each button becomes a column in a matrix, with the RHS becoming the desired light configuration
            // get to row-reduced echelon form with swaps + xor, identify free variables ( columns without a pivot )
            // then paramaterize for free variables and solve (or just iterate over all combinations of free variables which is 2^n)
            int answer = 0;
            ulong[][] machines = Input.FromFile("Inputs/Day10.txt").GetBitSets();
            foreach (ulong[] machine in machines)
            {
                Array.Sort(machine, (a, b) => b.CompareTo(a));
            }
            //WriteBitsets(machines);

            foreach (ulong[] machine in machines)
            {
                Console.WriteLine("=======================");
                ReduceToRref(machine);
                int presses = SolveSystem(machine);
                Console.WriteLine(presses);
                answer += presses;
            }
            Console.WriteLine(answer);
        }

        public static int SolveSystem(ulong[] bitsets)
        {
            List<ulong> freeVars = GetFreeVars(bitsets);


            int m = bitsets.Length;
            int n = bitsets.Select(BitCount).Max();
            int numVariables = n - 1;

            int minPresses = int.MaxValue;

            WriteBitSet(bitsets);

            if(freeVars.Count == 0)
            {
                return CountRhs(bitsets);
            }
            List<List<ulong>> combinations = GetAllFreeVarCombinations(freeVars);

            foreach(var combination in combinations)
            {
                Console.WriteLine("--");
                foreach(ulong freevar in combination)
                {
                    WriteBitSet(freevar, n);
                }
                int presses = GetMinPressesForMachine(bitsets, combination);
                if( presses < minPresses)
                {
                    minPresses = presses;
                }
            }
            return minPresses;
        }

        public static int GetMinPressesForMachine(ulong[] bitsets, List<ulong> freevars)
        {
            ulong[] matrix = (ulong[])bitsets.Clone();

            int n = bitsets.Length;
            for(int i = 0; i < n; i++)
            {
                ulong bitset = matrix[i];
                foreach(ulong freevar in freevars)
                {
                    int k = BitCount(freevar) - 1;
                    if(ReadBit(bitset, k))
                    {
                        bitset ^= freevar; 
                    }
                }
                matrix[i] = bitset;
            }
            WriteBitSet(matrix);
            int pressesFromFreeVars = 0;
            foreach(ulong freevar in freevars)
            {
                if(ReadBit(freevar, 0))
                {
                    pressesFromFreeVars++;
                }
            }
            return CountRhs(matrix) + pressesFromFreeVars;
        }

        public static int CountRhs(ulong[] bitsets)
        {
            return bitsets.Select(l => ReadBit(l, 0) ? 1 : 0).Sum();
        }
        public static List<List<ulong>> GetAllFreeVarCombinations(List<ulong> freevars)
        {
            int n = freevars.Count;
            List<List<ulong>> combinations = [];
            for(int i = 0; i < Math.Pow(2, n); i++)
            {
                List<ulong> combination = [.. freevars];
                for(int j = 0; j < n; j++)
                {
                    if((i >> j & 1) == 1)
                    {
                        ulong num = combination[j];
                        SetBit(ref num, 0);
                        combination[j] = num;
                    }
                }
                combinations.Add(combination);
            }
            return combinations;
        }

        public static List<ulong> GetFreeVars(ulong[] bitsets)
        {
            List<ulong> freeVars = [];
            int m = bitsets.Length;
            int n = bitsets.Select(BitCount).Max();

            int i = 0, j = n - 1;
            while(i < m)
            {
                ulong bitset = bitsets[i];
                if(bitset == 0)
                {
                    break;
                }
                if (ReadBit(bitset, j))
                {
                    j--;
                    i++; 
                }
                else
                {
                    ulong freevar = 0;
                    SetBit(ref freevar, j--);
                    freeVars.Add(freevar);
                }
            }
            while(j > 0)
            {
                ulong freevar = 0;
                SetBit(ref freevar, j--);
                freeVars.Add(freevar);
            }
            return freeVars;
            
        }
        public static int CountSetBits(ulong num)
        {
            int count = 0;
            while(num > 0)
            {
                if((num & 1UL) == 1)
                {
                    count++;
                }
                num >>= 1;
            }
            return count;
        }

        public static void ReduceToRref(ulong[] bitsets)
        {

            int n = bitsets.Select(BitCount).Max();
            int m = bitsets.Length;

            int j = 0;
            for (int i = n - 1; i > 0; i--)
            {
                if(j == m)
                {
                    break;
                }    
                if (!ReadBit(bitsets[j], i))
                {
                    if( j + 1 == m)
                    {
                        break;
                    }
                    int k = j + 1;
                    while (k < m && !ReadBit(bitsets[k], i))
                    {
                        k++;
                    }
                    if (k == m)
                    {
                        continue;
                    }
                    Swap(bitsets, j, k);
                }


                for (int k = j + 1; k < m; k++)
                {
                    if (ReadBit(bitsets[k], i))
                    {
                        bitsets[k] ^= bitsets[j];
                    }
                }
                j++;
            }

            for(int i = m - 1; i > 0; i--)
            {
                ulong bitset = bitsets[i];
                j = BitCount(bitset) - 1;
                if(j > 0)
                {
                    for(int k = i - 1; k >= 0; k--)
                    {
                        ulong row = bitsets[k];
                        if(ReadBit(row, j))
                        {
                            bitsets[k] ^= bitset;
                        }
                    }
                }
            }
        }

        public static void WriteBitsets(ulong[][] machines)
        {
            for(int i = 0; i < machines.Length; i++)
            {
                WriteBitSet(machines[i]);
            }
        }

        public static void WriteBitSet(ulong[] machine)
        {
            Console.WriteLine();
            int maxLength = 0;
            for (int j = 0; j < machine.Length; j++)
            {
                int l = Convert.ToString((long)machine[j], toBase: 2).Length;
                if (l > maxLength)
                {
                    maxLength = l;
                }
            }
            for (int j = 0; j < machine.Length; j++)
            {
                Console.WriteLine(Convert.ToString((long)machine[j], toBase: 2).PadLeft(maxLength, '0'));
            }
        }

        public static void WriteBitSet(ulong bitset, int length)
        {
            Console.WriteLine(Convert.ToString((long)bitset, toBase: 2).PadLeft(length, '0'));
        }
        public static ulong[][] GetBitSets(this Input input)
        {
            ulong[][] matrices = new ulong[input.Lines.Length][];
            for(int j = 0; j < input.Lines.Length; j++)
            {
                string line = input.Lines[j];
                int lBracketIdx = line.IndexOf('['), rBracketIdx = line.IndexOf(']');
                int n = rBracketIdx - lBracketIdx - 1;
                ulong[] bitsets = new ulong[n];
                int bitsetIdx = 0;
                for(int i = lBracketIdx + 1; i < rBracketIdx; i++)
                {
                    ulong bitset = 0UL;
                    char c = line[i];
                    if(c == '#')
                    {
                        bitset |= 1 << 0;
                    }
                    bitsets[bitsetIdx++] = bitset;
                }

                int lParenthesisIndex = line.IndexOf('('), rParenthesisIndex = line.LastIndexOf(')');
                string[] buttons = line.Substring(lParenthesisIndex, rParenthesisIndex - lParenthesisIndex).Split(' ');
                for(int i = 0; i < buttons.Length; i++)
                {
                    string button = buttons[i];
                    int[] linesToSet = [.. button.Trim('(').Trim(')').Split(',').Select(int.Parse)];
                    foreach(int l in linesToSet)
                    {
                        bitsets[l] |= 1UL << (i + 1);
                    }
                }
                matrices[j] = bitsets;
            }

            return matrices;
        }

        public static void Swap(ulong[] arr, int i1, int i2)
        {
            ulong temp = arr[i1];
            arr[i1] = arr[i2];
            arr[i2] = temp;
        }
        public static int BitCount(ulong value)
        {
            if (value == 0)
                return 0;
            return Convert.ToInt32(Math.Floor(Math.Log2(value))) + 1;
        }

        public static void SetBit(ref ulong value, int position)
        {
            value |= 1UL << position;
        }

        public static bool ReadBit(ulong value, int position)
        {
            return ((value >> position) & 1) == 1;
        }

        public static void XOr(ref ulong value, ulong bitmask)
        {
            value ^= bitmask;
        }

        #endregion

        public static int[][][] GetMatrices(this Input input)
        {
            int length = input.Lines.Length;
            int[][][] matrices = new int[length][][];
            for(int k = 0; k < length; k++)
            {
                string line = input.Lines[k];
                int leftParenthesisIdx = line.IndexOf('(');
                int rightParenthesisIdx = line.LastIndexOf(')');
                int buttonStringLength = rightParenthesisIdx - leftParenthesisIdx + 1;

                int leftBraceIndex = line.IndexOf('{');
                int rightBraceIndex = line.LastIndexOf('}');
                int joltageStringLength = rightBraceIndex - leftBraceIndex + 1; 

                List<List<int>> buttons = [.. line
                    .Substring(leftParenthesisIdx, buttonStringLength)
                    .Split(' ')
                    .Select(x =>
                    {
                        return x.Trim('(').Trim(')').Split(',').Select(int.Parse).ToList();
                    })
                ];

                List<int> joltages = [.. line
                    .Substring(leftBraceIndex, joltageStringLength)
                    .Trim('{')
                    .Trim('}')
                    .Split(',')
                    .Select(int.Parse)
                    .ToList()
                ];

                int m = joltages.Count, n = buttons.Count + 1;
                int[][] matrix = new int[m][];
                for(int i = 0; i < m; i++)
                {
                    matrix[i] = new int[n];
                }

                for(int i = 0; i < joltages.Count; i++)
                {
                    matrix[i][n - 1] = joltages[i];
                }
                
                for(int i = 0; i < n - 1; i++)
                {
                    List<int> button = buttons[i];
                    foreach(int num in button)
                    {
                        matrix[num][i] = 1;
                    }
                }
                matrices[k] = matrix;
            }
            return matrices;
        }

        public static void Solve2()
        {
            int[][][] matrices = Input.FromFile("Inputs/Day10.txt").GetMatrices();

            foreach (int[][] matrix in matrices)
            {
                Console.WriteLine("-----------------");
                WriteMatrix(matrix);
                Console.WriteLine();
                ReduceToRref(matrix);
                SolveSystem(matrix);
            }
        }

        public static int SolveSystem(int[][] matrix)
        {
            int[] freeVariables = GetFreeVariables(matrix);
            Console.WriteLine();
            WriteArray(freeVariables);
            Console.WriteLine();

            // At this point we have a matrix describing the effect of button press on the left and the target values on the RHS
            // Each free variable can be iterated from 0 -> f1 + f2 + .... + fn  s/t the target joltages are not exceeded by this number of button presses
            // Traversing the space of free variable values is similar to traversing a graph, so A* can be used
            // In this case a "node" is described by a complete set of free variable values and the adjacent nodes would be the set of 
            // sets of free button presses with exactly one value incremented by one
            //
            // initialize priority queue of free variable value arrays and populate it with starting value of [0, 0, .... , 0]
            // 
            // we have constraints that each variable must have value >= 0 - so we can enforce this constraint byh doing the following:
            // for each row in our matrix that has a free variable, we can take that row, solve for one of the bound variables, and get constraints on our remaining variables, 
            // and (hopefully) get constraints on our free variables this way
            // for each next candidate, we can 
            return 0;
        }

        public static int[] GetFreeVariables(int[][] matrix)
        {
            int m = matrix.Length;
            int n = matrix[0].Length - 1;

            int[] freeVariableArray = new int[n];
            int i = 0, j = 0;
            while(i < m && j < n)
            {
                if (matrix[i][j] != 0)
                {
                    i++;
                }
                else
                {
                    freeVariableArray[j] = 1;
                }
                j++;
            }
            while(j < n)
            {
                freeVariableArray[j++] = 1;
            }
            return freeVariableArray;

        }
        public static void ReduceToRref(int[][] matrix)
        {
            int m = matrix.Length;
            int n = matrix[0].Length - 1;
            int curRow = 0;
            for(int i = 0; i < n; i++)
            {
                if(curRow == m)
                {
                    break;
                }
                if (matrix[curRow][i] == 0 )
                {
                    for(int j = curRow + 1; j < m; j++)
                    {
                        if (matrix[j][i] != 0 )
                        {
                            Swap(matrix, j, curRow);
                            break;
                        }
                    }
                }


                if (matrix[curRow][i] == 0)
                {
                    continue;
                }

                for(int j = curRow + 1; j < m; j++)
                {
                    if (matrix[j][i] != 0)
                    {
                        int factorA = matrix[j][i];
                        int factorB = matrix[curRow][i];
                        for(int k = 0; k < n + 1; k++)
                        {
                            
                            matrix[j][k] = matrix[j][k] * factorB - factorA * matrix[curRow][k]; 
                        }
                    }
                }

                for(int j = curRow - 1; j >= 0; j--)
                {
                    if (matrix[j][i] != 0)
                    {
                        int factorA = matrix[j][i];
                        int factorB = matrix[curRow][i];
                        for(int k = 0; k < n + 1; k++)
                        {
                            matrix[j][k] = matrix[j][k] * factorB -  factorA * matrix[curRow][k];
                        }
                    }
                }

                //Console.WriteLine($"i = {i}, curRow = {curRow}");
                //WriteMatrix(matrix);
                curRow++;
            }
            Console.WriteLine();
            WriteMatrix(matrix);
        }

        public static void Swap(int[][] matrix, int r1, int r2)
        {
            int[] temp = matrix[r1];
            matrix[r1] = matrix[r2];
            matrix[r2] = temp;
        }
        public static void WriteMatrices(int[][][] matrices)
        {
            for(int i = 0; i < matrices.Length; i++) 
            {
                WriteMatrix(matrices[i]);
                Console.WriteLine();
            }
        }

        public static void WriteMatrix(int[][] matrix)
        {
            for (int j = 0; j < matrix.Length; j++)
            {
                WriteArray(matrix[j]);
                Console.WriteLine();
            }
        }

        public static void WriteArray(int[] array)
        {
            for (int k = 0; k < array.Length; k++)
            {
                Console.Write(array[k].ToString().PadLeft(3));
                if (k < array.Length - 1)
                {
                    Console.Write(',');
                }
            }
        }
    }
}
