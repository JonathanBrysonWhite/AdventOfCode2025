using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode25.Common
{
    public static class InputExtensions
    {
        public static string[][] Groups(this Input input)
        {
            return input.Raw
                .Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(g => g.Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                .ToArray();
        }

        public static int[] Ints(this Input input)
        {
            return input.Lines.Select(line => int.Parse(line)).ToArray();
        }
    }
}
