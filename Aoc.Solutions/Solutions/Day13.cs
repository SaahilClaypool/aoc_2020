using System.Collections.Generic;
using System.Linq;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions {
    public class Day13 : Day {
        record Input(int Start, List<int> Buses);
        static Input ParseInput(string input) {
            var lines = input.Split("\n").ToList();
            return new Input(
                int.Parse(lines[0]),
                lines[1].Split(",").Where(id => int.TryParse(id, out var _)).Select(int.Parse).ToList());
        }

        static int NextRun(int bus, int start) => start + bus - start % bus;

        public override string SolveA(string input) {
            var (start, buses) = ParseInput(input);
            var busTimes = buses
                .Select(bus => (bus, next: NextRun(bus, start)))
                .ToList();
            $"{string.Join(",", busTimes)}".Dbg();
            var min = busTimes.Aggregate((bus: int.MaxValue, next: int.MaxValue), (i, state) => (state.next < i.next) ? state : i);

            return (min.bus * (min.next - start)).ToString();
        }

        public override string SolveB(string input) {
            return "SolveB";
        }
        public Day13() {
            Tests = new()
            {
                new("sample A", "sample", "295", input => GetInput(input).Then(SolveA)),
                new("sample B", "sample", "1068781", input => GetInput(input).Then(SolveB))
            };

        }
    }
}
