using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Aoc.Runner;

using static Extensions.CommonExtensions;

namespace Aoc.Solutions.D25 {
    public class Day25 : Day {
        const uint MOD = 20201227;
        static (uint card, uint door) Parse(string input) {
            var lines = input.Split("\n").Select(uint.Parse).ToList();
            return (lines[0], lines[1]);
        }

        static long Transform(long subjectNumber, int loopSize) {
            var value = 1L;
            foreach (var _ in R(0..loopSize)) {
                value *= subjectNumber;
                value %= MOD;
            }
            return value;
        }

        // g^x % mod == h --> returns x
        static uint? BabyGiantStep(uint g, uint h, uint mod) {
            uint m = (uint)Math.Ceiling(Math.Sqrt(mod));
            var table = new Dictionary<uint, uint>();
            var e = 1L;

            // generate giant steps
            for (var i = 0U; i < m; i++) {
                table[(uint)e] = i;
                e = e * g % mod;
            }

            var factor = (uint)BigInteger.ModPow(g, mod - m - 1, mod);

            e = h;

            for (var i = 0u; i < m; i++) {
                if (table.TryGetValue((uint)e, out var val)) {
                    return i * m + val;
                }
                e = e * factor % mod;
            }
            return null;
        }

        // solve for  result(h) = subjectNumber(g) ^ loopSize(x) % mod(m)
        static uint FindLoopSize(uint subjectNumber, uint result) {
            return BabyGiantStep(g: subjectNumber, h: result, MOD)!.Value;
        }

        public override string SolveA(string input) {
            var (cardKey, doorKey) = Parse(input);
            var cardLoop = FindLoopSize(7, cardKey);
            // var doorLoop = FindLoopSize(7, doorKey);

            return Transform(doorKey, (int)cardLoop).ToString();
        }

        public override string SolveB(string input) {
            return "SolveB";
        }
        public Day25() {
            Tests = new()
            {
                new("1", "sample", "5764801", input => GetInput(input).Then(input => Transform(7, 8).ToString())),
                new("2", "sample", "17807724", input => GetInput(input).Then(input => Transform(7, 11).ToString())),
                new("3", "sample", "14897079", input => GetInput(input).Then(input => Transform(5764801, 11).ToString())),
                new("4", "sample", "14897079", input => GetInput(input).Then(input => Transform(17807724, 8).ToString())),
                new("5", "sample", "14897079", input => GetInput(input).Then(SolveA)),
                new("baby", "sample", "9", _ => BabyGiantStep(5, 22, 53).ToString()!),
            };

        }
    }
}
