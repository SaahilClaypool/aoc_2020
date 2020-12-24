using System.Collections.Generic;
using System.Linq;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions.D24 {
    enum Dir {
        E,
        SE,
        SW,
        W,
        NW,
        NE
    }

    record Loc(int X, int Y) {
        public IEnumerable<Loc> Adjacent() {
            yield return new(X - 2, Y);
            yield return new(X - 1, Y - 1);
            yield return new(X + 1, Y + 1);
            yield return new(X - 1, Y + 1);
            yield return new(X + 1, Y - 1);
            yield return new(X + 2, Y);
        }
    };

    class Solver {
        public IEnumerable<IEnumerable<Dir>> Input { get; set; }
        public DefaultDictionary<Loc, bool> Locations { get; set; } = new();
        public int Day = 0;

        public static Loc GetLoc(IEnumerable<Dir> line) {
            Loc start = new(0, 0);

            foreach (var d in line) {
                Loc offset = d switch {
                    Dir.E => new(2, 0),
                    Dir.SE => new(1, -1),
                    Dir.SW => new(-1, -1),
                    Dir.W => new(-2, 0),
                    Dir.NW => new(-1, 1),
                    Dir.NE => new(1, 1),
                    _ => throw new System.NotImplementedException(),
                };

                start = new(start.X + offset.X, start.Y + offset.Y);
            }

            return start;
        }

        public void Setup() {
            foreach (var line in Input) {
                var loc = GetLoc(line);
                Locations[loc] = !Locations[loc];
            }
        }

        public void FlipDay() {
            bool CheckNeighbors(Loc loc) {
                var tile = Locations[loc];
                return tile switch {
                    true => !(loc.Adjacent().Count(n => Locations[n]) is 0 or > 2),
                    false => loc.Adjacent().Count(n => Locations[n]) == 2
                };
            }
            var locationsWithAdjacent = Locations.Keys.SelectMany(l => l.Adjacent()).Concat(Locations.Keys).ToHashSet();
            var next = locationsWithAdjacent.ToDictionary(
                l => l,
                l => CheckNeighbors(l)
            );
            Locations = new DefaultDictionary<Loc, bool>(next);
            Day++;
        }
    }

    public class Day24 : Day {
        public override string SolveA(string input) {
            var solver = new Solver() { Input = Parse(input) };
            solver.Setup();
            return solver.Locations.Values.Count(isFlipped => isFlipped).ToString();
        }

        public override string SolveB(string input) {
            var solver = new Solver() { Input = Parse(input) };
            solver.Setup();
            foreach (var d in Enumerable.Range(0, 100)) {
                solver.FlipDay();
            }
            return solver.Locations.Values.Count(isFlipped => isFlipped).ToString();
        }

        static IEnumerable<IEnumerable<Dir>> Parse(string input) => input.Split("\n").Select(ParseLine);
        static IEnumerable<Dir> ParseLine(string input) {
            for (var i = 0; i < input.Length; i++) {
                var longDir = i < input.Length - 1 ? input.Substring(i, 2) : input.Substring(i, 1);
                if (longDir is "se") { i++; yield return Dir.SE; }
                else if (longDir is "sw") { i++; yield return Dir.SW; }
                else if (longDir is "ne") { i++; yield return Dir.NE; }
                else if (longDir is "nw") { i++; yield return Dir.NW; }
                else {
                    yield return input[i] switch {
                        'e' => Dir.E,
                        'w' => Dir.W,
                        _ => throw new System.Exception("unknown letter")
                    };
                }
            }
        }

        public Day24() {
            Tests = new()
            {
                new("1", "nwwswee", "[\"nw\",\"w\",\"sw\",\"e\",\"e\"]", input => ParseLine(input).ToList().ToJson()),
                new("ref", "nwwswee", "Loc { X = 0, Y = 0 }", input => ParseLine(input).Then(Solver.GetLoc).Then(t => t.ToString())),
                new("adj", "esew", "Loc { X = 1, Y = -1 }", input => ParseLine(input).Then(Solver.GetLoc).Then(t => t.ToString())),
                new("long", "sesenwnenenewseeswwswswwnenewsewsw", "Loc { X = -4, Y = -2 }", input => ParseLine(input).Then(Solver.GetLoc).Then(t => t.ToString())),
                new("sample", "sample", "10", input => GetInput(input).Then(SolveA)),
                new("sample", "sample", "2208", input => GetInput(input).Then(SolveB)),
            };

        }
    }
}
