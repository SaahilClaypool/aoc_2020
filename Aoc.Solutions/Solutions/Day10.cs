using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions {
    public class Day10 : Day {
        static List<long> Jolts(string input) {
            var jolts = input.Split("\n").Select(long.Parse).ToList().Then(l => { l.Sort(); return l; });
            jolts = jolts.Prepend(0).ToList();
            jolts.Add(jolts.Max() + 3);
            return jolts;
        }
        public override string SolveA(string input) {
            var jolts = Jolts(input);
            var differences = new List<long>();
            foreach (var i in Enumerable.Range(0, jolts.Count - 1)) {
                differences.Add(jolts[i + 1] - jolts[i]);
            }
            var groupCounts = differences.GroupBy(d => d).ToDictionary(g => (int)g.Key, g => g.Count());
            groupCounts.Dbg();

            var diff1 = groupCounts[1];
            var diff3 = groupCounts[3];
            return (diff1 * diff3).ToString();
        }

        public override string SolveB(string input) {
            var jolts = Jolts(input);
            jolts.Reverse();
            var subtreeCounts = new Dictionary<long, long>();
            foreach (var jolt in jolts) {
                var possibleNext = jolts.Where(j => j > jolt && j <= jolt + 3);
                subtreeCounts[jolt] = possibleNext.Select(n => subtreeCounts[n]).Sum();
                if (subtreeCounts[jolt] == 0) subtreeCounts[jolt] = 1;
            }
            return subtreeCounts[0].ToString();
        }
        public Day10() {
            Tests = new()
            {
                new("1", "long", "220", input => GetInput(input).Then(SolveA)),
                new("b", "long", "19208", input => GetInput(input).Then(SolveB)),
                new("bshort", "short", "8", input => GetInput(input).Then(SolveB))
            };

        }
    }
}
