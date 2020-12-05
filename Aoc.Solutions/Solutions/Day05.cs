using System;
using System.Collections.Generic;
using System.Linq;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions {
    public class Day05 : Day {


        record SearchState(int Min, int Max);
        record State(SearchState Row, SearchState Col);

        static int Midpoint(SearchState st) => (st.Min + st.Max) / 2;
        static SearchState Upper(SearchState st) => st with { Min = Midpoint(st) + 1 };
        static SearchState Lower(SearchState st) => st with { Max = Midpoint(st) };
        static (int, int) Search(string seat) {
            var seatPostition =
                seat.Aggregate(new State(new(0, 127), new(0, 7)),
                    (state, c) => c switch {
                        'F' => state with { Row = Lower(state.Row) },
                        'B' => state with { Row = Upper(state.Row) },
                        'R' => state with { Col = Upper(state.Col) },
                        'L' => state with { Col = Lower(state.Col) },
                        _ => state
                    });

            return (Midpoint(seatPostition.Row), Midpoint(seatPostition.Col));
        }

        static int Sid((int, int) seat) => seat.Item1 * 8 + seat.Item2;

        public override string SolveA(string input) =>
            input.Split("\n")
                .Select(Search)
                .Select(Sid)
                .Max()
                .ToString();

        static IEnumerable<(int, int)> AllSeats() {
            foreach (var r in Enumerable.Range(0, 128))
                foreach (var c in Enumerable.Range(0, 8))
                    yield return (r, c);
        }

        public override string SolveB(string input) {
            var seats = input.Split("\n")
                .Select(Search)
                .ToHashSet();
            var ids = seats.Select(Sid).ToHashSet();
            return AllSeats().First(s =>
                !seats.Contains(s)
                && ids.Contains(Sid(s) - 1)
                && ids.Contains(Sid(s) + 1)
            ).Then(Sid).ToString();


        }
        public Day05() {
            Tests = new()
            {
                new("BFFFBBFRRR", "BFFFBBFRRR", "(70, 7)", input => Search(input).ToString())
            };
        }

    }
}