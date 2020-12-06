using System.Collections.Generic;
using System.Linq;
using Extensions;

using Aoc.Runner;

namespace Aoc.Solutions {
    public class Day06 : Day {
        static IEnumerable<HashSet<char>> ParseA(string input) =>
            input
            .Split("\n")
            .SplitAt(line => line.Length == 0)
            .Select(block => {
                var set = new HashSet<char>();
                foreach (var line in block) {
                    foreach (var c in line) {
                        set.Add(c);
                    }
                }
                return set;
            });
        public override string SolveA(string input) =>
            ParseA(input)
                .Aggregate(0, (acc, set) => acc + set.Count)
                .ToString();

        static IEnumerable<IEnumerable<char>> ParseB(string input) =>
            input
            .Split("\n")
            .SplitAt(line => line.Length == 0)
            .Select(block =>
                block
                    .SelectMany(line => line)
                    .GroupBy(c => c)
                    .Where(g => g.Count() == block.Count())
                    .Select(g => g.Key)
            );
        public override string SolveB(string input) =>
            ParseB(input)
                .Aggregate(0, (acc, block) => acc + block.Count())
                .ToString();
        public Day06() {
            Tests = new()
            {
                new("1", "input", "SolveA", (input) => SolveA(input))
            };

        }
    }
}
