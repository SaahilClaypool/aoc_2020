using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions {
    public class Day10 : Day {
        static List<BigInteger> Jolts(string input) {
            var jolts = input.Split("\n").Select(BigInteger.Parse).ToList().Then(l => { l.Sort(); return l; });
            jolts = jolts.Prepend(0).ToList();
            jolts.Add(jolts.Max() + 3);
            return jolts;
        }
        public override string SolveA(string input) {
            var jolts = Jolts(input);
            var differences = new List<BigInteger>();
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
            BigInteger count = 1;
            var jolts = Jolts(input);
            jolts.Reverse();
            var waysToReach = new List<BigInteger>();
            foreach (var jolt in jolts) {
            }
            return count.ToString();
        }
        public Day10() {
            Tests = new()
            {
                new("1", "long", "220", input => GetInput(input).Then(SolveA)),
                new("b", "long", "19208", input => GetInput(input).Then(SolveB))
            };

        }
    }
}
