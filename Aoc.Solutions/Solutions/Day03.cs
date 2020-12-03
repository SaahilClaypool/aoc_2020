using System.Collections.Generic;
using System.Linq;

using Aoc.Runner;

namespace Aoc.Solutions {
    public class Day03 : Day {
        public int SolveWithSlope(string input, int dx, int dy) {
            var x = 0;
            var hits = input.Split("\n")
                .Where(line => {
                    var hit = line[x] == '#';
                    x += dx;
                    x %= line.Length;
                    return hit;
                }
               ).Count();
            return hits;
        }

        public override string SolveA(string input) =>
            SolveWithSlope(input, 3, 1).ToString();

        public override string SolveB(string input) {
            return "SolveB";
        }
        public Day03() {
            Tests = new()
            {
                new("1", "input", "SolveA", (input) => SolveA(input))
            };

        }
    }
}
