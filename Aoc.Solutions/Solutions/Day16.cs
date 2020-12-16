using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions {
    public class Day16 : Day {


        record Range(int Min, int Max);
        record Rule(string Name, List<Range> Ranges);
        class Ticket : List<int> { public Ticket(List<int> ticket) : base(ticket) { } };
        record Input(List<Rule> Rules, Ticket MyTicket, List<Ticket> OtherTickets);
        Input ParseInput(string input) {
            var zones = input.Split("\n\n").ToList();
            var rules = zones[0].Split("\n").Select(ParseRule).ToList();
            var myTicket = zones[1].Split("\n").Skip(1).First().Then(ParseTicket);
            var otherTickets = zones[2].Split("\n").Skip(1).Select(ParseTicket).ToList();
            return new Input(rules, myTicket, otherTickets);
        }

        Ticket ParseTicket(string line) => line.Split(",").Select(int.Parse).ToList().Then(list => new Ticket(list));

        Rule ParseRule(string rule) {
            var parts = rule.Split(":");
            var name = parts[0].Trim();
            var ranges = new Regex(@"(?<min>\d+)-(?<max>\d+)").Matches(parts[1])
                .Select(m => new Range(int.Parse(m.Groups["min"].Value), int.Parse(m.Groups["max"].Value)))
                .ToList();
            return new Rule(name, ranges);
        }

        public override string SolveA(string inputString) {
            var input = ParseInput(inputString);
            input.Dbg();
            return "SolveA";
        }

        public override string SolveB(string input) {
            return "SolveB";
        }
        public Day16() {
            Tests = new()
            {
                new("1", "input", "SolveA", (input) => SolveA(input))
            };

        }
    }
}
