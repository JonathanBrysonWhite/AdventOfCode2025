using AdventOfCode25.Solutions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode25.Common
{
    internal static class StringExtensions
    {
        public static Operation ToOperation(this string s)
        {
            switch (s)
            {
                case "*": return Operation.Multiply;
                case "+": return Operation.Add;
                default: return Operation.Add;
            }
        }
    }
}
