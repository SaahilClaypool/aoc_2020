using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions {
    public class Day13 : Day {
        record Input(int Start, List<int?> Buses);
        static Input ParseInput(string input) {
            var lines = input.Split("\n").ToList();
            return new Input(
                int.Parse(lines[0]),
                lines[1].Split(",").Select(id => {
                    int? result = int.TryParse(id, out var v) switch {
                        true => v,
                        false => null
                    };
                    return result;
                }
                ).ToList());
        }

        static int NextRun(int bus, int start) => start + bus - start % bus;

        public override string SolveA(string input) {
            var (start, buses) = ParseInput(input);
            var busTimes = buses
                .Where(bus => bus is not null)
                .Select(bus => bus!.Value)
                .Select(bus => (bus, next: NextRun(bus, start)))
                .ToList();
            $"{string.Join(",", busTimes)}".Dbg();
            var min = busTimes.Aggregate((bus: int.MaxValue, next: int.MaxValue), (i, state) => (state.next < i.next) ? state : i);

            return (min.bus * (min.next - start)).ToString();
        }

        static bool StartTimeValid(BigInteger start, List<int?> buses) {
            if (start % 100000000000000 == 0) "Reached 100000000000000".Dbg();
            if (start.CompareTo(0) < 0) throw new System.Exception($"Wrapped at {start}");
            static bool RunsOn(int busId, BigInteger offset) => offset % busId == 0;
            return buses
                .Select((bus, number) => (bus, number))
                .All(bus => {
                    var result = bus.bus is null || RunsOn(bus.bus!.Value, start + bus.number);
                    return result;
                });
        }

        static BigInteger ChineseRemainder(List<(int bus, int number)> buses) {
            var remainders = buses
                .Select(bus => (bus.bus, (bus.bus - bus.number) % bus.bus))
                .OrderByDescending(bus => bus.bus)
                .ToList();
            // TODO: ensure GCD is 1
            var product = buses.Select(bus => bus.bus).Aggregate(new BigInteger(1), (acc, bus) => acc * bus);
            var result = new BigInteger(0);
            foreach (var (bus, remainder) in remainders) {
                var productWithoutCurrent = product / bus;
                var modInverse = BigInteger.ModPow(productWithoutCurrent, bus - 2, bus);
                var busIdResult = remainder * modInverse * productWithoutCurrent;
                result += busIdResult;
            }
            return result % product;
        }

        static long SolveBruteForce(List<(int bus, int number)> busAndIndex) {
            long start = 1;
            long cycleSize = 1;
            foreach (var (bus, number) in busAndIndex) {
                var remainderInCycle = (bus - number) % bus;
                var xyz = 1;
                while (start % bus - remainderInCycle != 0) {
                    if (start < 0L) throw new System.Exception("Wrapped...");
                    if (xyz++ % 1000000 == 0) System.Console.WriteLine(start);
                    start += cycleSize;
                }
                cycleSize *= bus; // going to need to step by at least the size of this bus...
                System.Console.WriteLine($"Bus {bus} at {number} would fit in at {start}. Cycle at {cycleSize}");
            }
            return start;
        }

        public override string SolveB(string input) {
            var (_, buses) = ParseInput(input);

            var busAndIndex = buses
                .Select((bus, number) => (bus, number))
                .Where(busnum => busnum.bus is not null)
                .Select(bus => (bus: bus.bus!.Value, bus.number))
                .OrderByDescending(bus => bus.bus)
                .ToList();


            return ChineseRemainder(busAndIndex).ToString();
        }

        public Day13() {
            Tests = new()
            {
                new("sample A", "sample", "295", input => GetInput(input).Then(SolveA)),
                new("sample B", "sample", "1068781", input => GetInput(input).Then(SolveB)),
                new("sample 2 B", "sample2", "3417", input => GetInput(input).Then(SolveB)),
                new("67,7,59,61", "1\n67,7,59,61", "754018", input => input.Then(SolveB)),
                new("67,x,7,59,61", "1\n67,x,7,59,61", "779210", input => input.Then(SolveB)),
                new("67,7,x,59,61", "1\n67,7,x,59,61", "1261476", input => input.Then(SolveB)),
                new("1789,37,47,1889", "1\n1789,37,47,1889", "1202161486", input => input.Then(SolveB))
            };

        }
    }
}
