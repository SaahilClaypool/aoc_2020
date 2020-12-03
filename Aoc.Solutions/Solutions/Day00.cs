using System.Collections.Generic;
using System.Linq;

using Aoc.Runner;

namespace Aoc.Solutions {
    public class Day00 : Day {
        public override string SolveA(string input) {
            return "SolveA";
        }

        public override string SolveB(string input) {
            return "SolveB";
        }
        public Day00() {
            Tests = new()
            {
                new("1", "input", "SolveA", (input) => SolveA(input))
            };

        }
    }
}
