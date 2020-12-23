using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions.D23 {
    // doubly linked list node
    class Cup {
        [JsonIgnore]
        public Cup Left { get; set; }
        [JsonIgnore]
        public Cup Right { get; set; }
        public int Label { get; set; }
        public bool InRing = true;

        public void Link(Cup other) {
            Right = other;
            other.Left = this;
        }
    }

    class Solver {
        public Cup Current { get; set; }
        public Dictionary<int, Cup> Cups { get; } = new();
        public int MinCup { get; init; }
        public int MaxCup { get; set; }

        public int ModLabel(int number) {
            var range = MaxCup - MinCup + 1;
            return (number - MinCup + range) % range + MinCup;
        }

        public void Move() {
            // 1. remove from ring
            var cup1 = Current.Right;
            var cup2 = cup1.Right;
            var cup3 = cup2.Right;
            foreach (var cup in new[] { cup1, cup2, cup3 }) {
                cup.InRing = false;
            }
            Current.Link(cup3.Right);

            // 2. Destination
            var label = ModLabel(Current.Label - 1);
            while (!Cups[label].InRing) {
                label = ModLabel(label - 1);
            }
            var destination = Cups[label];

            // 3. Insert cups
            var oldRight = destination.Right;
            destination.Link(cup1);
            cup3.Link(oldRight);
            foreach (var cup in new[] { cup1, cup2, cup3 }) {
                cup.InRing = true;
            }

            // 4. Select new current
            Current = Current.Right;
        }

        public string Show() {
            string CupNum(Cup cup) => cup == Current ? $"({cup.Label})" : $" {cup.Label} ";
            var cupBuffer = "";
            var start = Cups.Values.First(c => c.InRing);
            cupBuffer += CupNum(start);
            var current = start.Right;
            while (start != current) {
                cupBuffer += CupNum(current);
                current = current.Right;
            }
            return $"cups: {cupBuffer}\npick up: {string.Join(",", Cups.Values.Where(c => !c.InRing).Select(c => c.Label))}";
        }
    }

    public class Day23 : Day {
        static Solver Parse(string content) {
            var labels = content
                .ToCharArray()
                .Select(c => int.Parse($"{c}"))
                .ToList();
            var maxCup = labels.Max();
            var minCup = labels.Min();

            var solver = new Solver() { MaxCup = maxCup, MinCup = minCup };
            foreach (var cupLabel in labels) {
                solver.Cups.Add(cupLabel, new() { Label = cupLabel });
            }
            foreach (var cupIndex in Enumerable.Range(0, labels.Count)) {
                var label = labels[cupIndex];
                var leftLabel = labels[(cupIndex - 1 + labels.Count) % labels.Count];
                var rightLabel = labels[(cupIndex + 1 + labels.Count) % labels.Count];
                solver.Cups[label].Left = solver.Cups[leftLabel];
                solver.Cups[label].Right = solver.Cups[rightLabel];
            }
            solver.Current = solver.Cups[labels[0]];
            return solver;
        }

        public override string SolveA(string input) {
            return SolveA(input, 100);
        }

        public string SolveA(string input, int rounds) {
            var solver = Parse(input);
            foreach (var _ in Enumerable.Range(0, rounds)) {
                solver.Move();
            }

            var buffer = "";
            var start = solver.Cups[1];
            var current = start.Right;
            while (current != start) {
                buffer += current.Label;
                current = current.Right;
            }

            return buffer;
        }

        public override string SolveB(string input) {
            return SolveB(input, 10000000);
        }

        // probably could check for seen states
        public string SolveB(string input, int rounds) {
            var solver = Parse(input);
            var first = solver.Current;
            var current = solver.Current.Left;
            var idx = solver.MaxCup;

            while (solver.Cups.Count < 1000000) {
                idx += 1;
                var cup = new Cup() { Label = idx };
                solver.Cups.Add(idx, cup);
                current.Link(cup);
                cup.Link(first);
                solver.MaxCup = idx;
                current = cup;
            }

            foreach (var _ in Enumerable.Range(0, rounds)) {
                solver.Move();
            }

            var one = solver.Cups[1];
            var rightCup = one.Right;
            var secondRightCup = one.Right.Right;
            var result = (long)rightCup.Label * secondRightCup.Label;


            $"{rightCup.Label} {secondRightCup.Label} {result}".Dbg();

            return result.ToString();
        }
        public Day23() {
            Tests = new()
            {
                new("1", "389125467", "92658374", input => SolveA(input, 10)),
                new("1", "389125467", "149245887792", input => SolveB(input))
            };

        }
    }
}
