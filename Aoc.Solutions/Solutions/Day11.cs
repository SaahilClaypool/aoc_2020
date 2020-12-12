using System;
using System.Collections.Generic;
using System.Linq;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions {

    public class Day11 : Day {
        record Seat((int row, int col) Loc, char State);

        Dictionary<(int row, int col), Seat> Seats = new();

        Dictionary<(int row, int col), Seat> ParseInput(string input) =>
            input.Split("\n")
            .SelectMany((line, row) =>
                line.ToCharArray().Select((state, Column) =>
                    new Seat((row, Column), state))
            ).ToDictionary(s => s.Loc, s => s);

        IEnumerable<Seat> Adjacent(Seat seat) {
            foreach (var r in Enumerable.Range(seat.Loc.row - 1, 3))
                foreach (var c in Enumerable.Range(seat.Loc.col - 1, 3)) {
                    if (r == seat.Loc.row && c == seat.Loc.col) continue;
                    if (Seats.ContainsKey((r, c))) yield return Seats[(r, c)];
                }
        }

        IEnumerable<Seat> Visible(Seat seat) {
            var directions = new List<(int row, int col)>() {
                (-1, -1), (-1, 0), (-1, 1),
                (0, -1), (0, 1),
                (1, -1), (1, 0), (1, 1)
            };


            foreach (var dir in directions) {
                var loc = Step(seat.Loc, dir);
                while (Seats.ContainsKey(loc)) {
                    if (Seats[loc].State != '.') {
                        yield return Seats[loc];
                        break;
                    }
                    loc = Step(loc, dir);
                }
            }
        }

        (int row, int col) Step((int row, int col) loc, (int row, int col) dir) =>
            (loc.row + dir.row, loc.col + dir.col);

        Seat NextStateA(Seat seat) {
            var adjacent = Adjacent(seat);
            var occupiedCount = adjacent.Where(s => s.State == '#').Count();
            if (seat.State == 'L' && occupiedCount == 0) {
                return seat with { State = '#' };
            }
            else if (seat.State == '#' && occupiedCount >= 4) {
                return seat with { State = 'L' };
            }
            return seat;
        }

        void Output() {
            var rows = Seats.Keys.Select(k => k.row).Max();
            var cols = Seats.Keys.Select(k => k.col).Max();
            foreach (var r in Enumerable.Range(0, rows + 1)) {
                foreach (var c in Enumerable.Range(0, cols + 1)) {
                    System.Console.Write(Seats[(r, c)].State);
                }
                System.Console.WriteLine();
            }
            System.Console.WriteLine("------------------------------");
        }

        string Simulate(string input, Func<Seat, Seat> next) {
            Seats = ParseInput(input);
            bool hasChanged = true;
            while (hasChanged) {
                hasChanged = false;
                Dictionary<(int row, int col), Seat> nextSeats = new();
                foreach (var seat in Seats.Values) {
                    if (seat.State == '.') {
                        nextSeats.Add(seat.Loc, seat);
                        continue;
                    }

                    var nextSeat = next(seat);
                    nextSeats.Add(nextSeat.Loc, nextSeat);
                    if (nextSeat != seat) {
                        hasChanged = true;
                    }
                }
                Seats = nextSeats;
            }

            return Seats.Values.Where(seat => seat.State == '#').Count().ToString();

        }

        Seat NextStateB(Seat seat) {
            var adjacent = Visible(seat);
            var occupiedCount = adjacent.Where(s => s.State == '#').Count();
            if (seat.State == 'L' && occupiedCount == 0) {
                return seat with { State = '#' };
            }
            else if (seat.State == '#' && occupiedCount >= 5) {
                return seat with { State = 'L' };
            }
            return seat;
        }

        public override string SolveA(string input) => Simulate(input, NextStateA);
        public override string SolveB(string input) => Simulate(input, NextStateB);

        public Day11() {
            Tests = new()
            {
                new("1", "sample", "37", (input) => GetInput(input).Then(SolveA)),
                new("B", "sample", "26", (input) => GetInput(input).Then(SolveB)),
                new("small", "small", "8", input => GetInput(input).Then(input => {
                    Seats = ParseInput(input);
                    var empty = Seats.Values.First(s => s.State == 'L');
                    var vis = Visible(empty);
                    foreach (var v in vis) {
                        $"{v}".Dbg();
                    }
                    return vis.Where(v => v.State == '#').Count().ToString();
                })),
                new("blind", "blind", "0", input => GetInput(input).Then(input => {
                    Seats = ParseInput(input);
                    var empty = Seats.Values.First(s => s.State == 'L');
                    var vis = Visible(empty);
                    foreach (var v in vis) {
                        $"{v}".Dbg();
                    }
                    return vis.Where(v => v.State == '#').Count().ToString();
                }))
            };

        }
    }
}
