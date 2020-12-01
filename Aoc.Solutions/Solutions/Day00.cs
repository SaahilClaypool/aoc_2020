using System.Collections.Generic;
using Aoc.Runner;

namespace Aoc.Solutions
{
    public class Day00 : Day
    {
        public Day00()
        {
            Tests = new()
            {
                new("1", "input", "SolveA", (input) => SolveA(input))
            };

        }
        public override string SolveA(string input)
        {
            return "SolveA";
        }

        public override string SolveB(string input)
        {
            return "SolveB";
        }
    }
}