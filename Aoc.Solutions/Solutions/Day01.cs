using System.Collections.Generic;
using System.Linq;

using Aoc.Runner;

namespace Aoc.Solutions {
    public class Day01 : Day {

        public override string SolveA(string input) {
            var target = 2020;
            var numbers = input.Split("\n").Select(l => int.Parse(l.Trim())).ToList();
            numbers.Sort();
            // walk each direction to find numbers that add to the thing I want
            var (l, r) = (0, numbers.Count - 1);
            var val = 0;
            while ((val = numbers[l] + numbers[r]) != target) {
                if (val < target) {
                    l += 1;
                }
                else {
                    r -= 1;
                }
            }
            return (numbers[l] * numbers[r]).ToString();
        }

        public override string SolveB(string input) {
            var target = 2020;
            var numbers = input.Split("\n").Select(l => int.Parse(l.Trim())).ToList();
            foreach (var (i, j, k) in Combinations(numbers)) {
                if (i + j + k == target) {
                    return (i * j * k).ToString();
                }
            }
            return "SolveB";
        }


        public static IEnumerable<(T, T, T)> Combinations<T>(List<T> items) {
            for (var i = 0; i < items.Count - 2; i++)
                for (var j = i + 1; j < items.Count - 1; j++)
                    for (var k = j + 1; k < items.Count; k++) {
                        yield return (items[i], items[j], items[k]);
                    }
        }
        public Day01() {
            Tests = new()
            {
                new("1", @"1721
                                979
                                366
                                299
                                675
                                1456", "514579", (input) => SolveA(input))
            };

        }
    }

}
