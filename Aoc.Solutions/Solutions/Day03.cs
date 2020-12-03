using System.Collections.Generic;
using System.Linq;

using Aoc.Runner;

namespace Aoc.Solutions {
    public class Day03 : Day {
        public static int SolveWithSlope(string input, int dx, int dy) {
            var x = 0;
            var hits = input.Split("\n")
                .Where((line, idx) => {
                    if (idx % dy != 0) { // slope > 1 --> only count on dy'th row
                        return false;
                    }

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
            List<List<int>> slopes = new() {
                new() { 1, 1 },
                new() { 3, 1 },
                new() { 5, 1 },
                new() { 7, 1 },
                new() { 1, 2 }
            };

            return slopes.Select(slope =>
                SolveWithSlope(input, slope[0], slope[1])
            ).Aggregate(1L, (acc, numberHit) => acc * numberHit)
            .ToString();
        }
        public Day03() {
            Tests = new()
            {
                new("1", "input", "SolveA", (input) => SolveA(input))
            };

        }
    }
}
