using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode25.Common
{
    public sealed class Input
    {
        public string Raw { get; }
        public string[] Lines { get; }

        private Input(string raw)
        {
            Raw = raw;
            Lines = raw.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        }

        private Input(string raw, StringSplitOptions options)
        {
            Raw = raw;
            Lines = raw.Split("\r\n", options);
        }

        public static Input FromFile(string path)
        {
            var raw = File.ReadAllText(path);
            return new Input(raw);
        }

        public static Input FromFile(string path, StringSplitOptions options)
        {
            var raw = File.ReadAllText(path);
            return new Input(raw, options);
        }
    }
}
