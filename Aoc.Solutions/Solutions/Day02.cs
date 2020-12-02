using System.Collections.Generic;
using System.Linq;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions {
    record Line(int Min, int Max, char C, string Password) {
        public bool Valid() {
            var count = Password.Where(c => c == C).Count();
            return count <= Max && count >= Min;
        }

        public bool ValidB() {
            return Password[Min - 1] == C ^ Password[Max - 1] == C;
        }
    };
    public class Day02 : Day {
        public override string SolveA(string input) {
            var valid = input.Split("\n")
                .Select(l => ParseLine(l))
                .Where(l => l.Valid())
                .Count();
            return valid.ToString();
        }

        public override string SolveB(string input) {
            var valid = input.Split("\n")
                .Select(l => ParseLine(l))
                .Where(l => l.ValidB())
                .Count();
            return valid.ToString();
        }

        static Line ParseLine(string line) {
            var parts = line.Split(" ");
            var range = parts[0];
            var letter = parts[1][0];
            var password = parts[2];

            return new Line(
                int.Parse(range.Split("-")[0]),
                int.Parse(range.Split("-")[1]),
                letter,
                password
            );

        }


        public Day02() {
            Tests = new()
            {
                new("B", "1-3 a: abcde", "1", (input) => SolveB(input))
            };

        }
    }
}
