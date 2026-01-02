using AdventOfCode25.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode25.Solutions
{
    internal class Dial
    {
        public int position;
        public int clicks;

        public Dial()
        {
            position = 50;
            clicks = 0;
        }

        public void Rotate(Rotation rotation)
        {
            int value = rotation.Value;
            int starting = position;
            if(rotation.Dir == "L")
            {
                value *= -1;
            }
            position += value;

            if(position <= 0 && starting > 0)
            {
                clicks++;
            }
            clicks += Math.Abs(position / 100);
                position %= 100;
            if (position < 0)
                position = 100 + position;
        }

        public override string ToString()
        {
            return $"Position: {position}";
        }
    }

    public record Rotation(string Dir, int Value);



    internal static class Day1
    {
        public static Rotation[] Rotations(this Input input)
        {
            return input.Lines.Select(l =>
            {
                return new Rotation(l[0].ToString(), int.Parse(l.Substring(1)));
            })
            .ToArray();
        }

        public static void Solve()
        {
            int solution = 0;
            Dial dial = new Dial();
            Input input = Input.FromFile("Inputs/Day1-1.txt");
            Rotation[] rotations = input.Rotations();
            foreach(Rotation r in rotations)
            {
                Console.WriteLine(dial.ToString() + $"\tDir: {r.Dir}\tValue: {r.Value}\tClicks:{dial.clicks}");
                dial.Rotate(r);
                if(dial.position == 0)
                {
                    solution++;
                }
            }
            Console.WriteLine(solution);
            Console.WriteLine(dial.clicks);
        }
    }
}
