using System.Collections.Generic;
using System.Linq;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions {
    public class Day15 : Day {
        static List<int> ParseInput(string input) => input.Split(",").Select(i => int.Parse(i)).ToList();
        public string SolveA(string input, int max) {
            var said = new List<int>();
            var lookup = new Dictionary<int, List<int>>();
            var current = 1;

            List<int> Lookup(int number) {
                if (lookup.TryGetValue(number, out var val))
                    return val;
                lookup[number] = new List<int>();
                return lookup[number];
            }

            foreach (var initial in ParseInput(input)) {
                said.Add(initial);
                Lookup(initial).Add(current);
                current++;
            }

            while (current <= max) {
                var lastNumber = said[^1];
                var lastSaidAt = Lookup(lastNumber);
                var say = 0;
                if (lastSaidAt.Count >= 2) {
                    var lastSaid = lastSaidAt[^1];
                    var lastSaidBefore = lastSaidAt[^2];
                    say = lastSaid - lastSaidBefore;
                }
                // $"{current}: Saying {say}, {lastNumber} {lastSaidAt.ToJson()}".Dbg();
                said.Add(say);
                Lookup(say).Add(current);
                current++;
            }

            return said[^1].ToString();
        }

        public override string SolveA(string input) =>
            SolveA(input, 2020);

        public override string SolveB(string input) {
            return SolveA(input, 30000000);
        }
        public Day15() {
            Tests = new()
            {
                new("0,3,6", "0,3,6", "0", input => SolveA(input, 10)),
                new("0,3,6", "0,3,6", "436", input => SolveA(input)),
                new("1,3,2", "1,3,2", "1", input => SolveA(input)),
                new("2,1,3", "2,1,3", "10", input => SolveA(input)),
                new("1,2,3", "1,2,3", "27", input => SolveA(input))
            };

        }
    }
}
