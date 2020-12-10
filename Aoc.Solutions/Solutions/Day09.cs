using System;
using System.Collections.Generic;
using System.Linq;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions {
    public class Day09 : Day {
        static bool HasSum(long number, IEnumerable<long> preamble) {
            var sorted = preamble.ToList();
            sorted.Sort();
            var (l, r) = (0, sorted.Count - 1);

            while (r > l) {
                var sum = sorted[l] + sorted[r];
                if (sum > number)
                    r--;
                else if (sum < number)
                    l++;
                else
                    return true;
            }

            return false;
        }

        static long? FirstFailure(List<long> numbers, int preambleLength) {
            foreach (var i in Enumerable.Range(0, numbers.Count)) {
                if (i < preambleLength) {
                    continue;
                }
                var preamble = numbers.GetRange(i - preambleLength, preambleLength);
                if (!HasSum(numbers[i], preamble))
                    return numbers[i];
            }
            return null;
        }

        public override string SolveA(string input) => SolveA(input, 25);
        static string SolveA(string input, int preambleLength) {
            var numbers = input.Split("\n").Select(l => long.Parse(l)).ToList();
            return FirstFailure(numbers, preambleLength).ToString()!;
        }

        public override string SolveB(string input) => SolveB(input, 675280050);
        static string SolveB(string input, long target) {
            var numbers = input.Split("\n").Select(l => long.Parse(l)).ToList();

            foreach (var i in Enumerable.Range(0, numbers.Count - 1)) {
                foreach (var j in Enumerable.Range(i + 1, numbers.Count - i - 1)) {
                    var range = numbers.GetRange(i, j - i);
                    var sum = range.Sum();
                    if (sum == target) {
                        return (range.Min() + range.Max()).ToString();
                    }
                }
            }

            return "none";
        }

        public Day09() {
            Tests = new()
            {
                new("sample 5", "sample", "127", input => GetInput(input).Then(i => SolveA(i, 5))),
                new("sample 5", "sample", "62", input => GetInput(input).Then(i => SolveB(i, 127)))
            };

        }
    }
}
