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

        static bool Matches(int field, Rule rule) =>
            rule.Ranges.Any(range => range.Min <= field && range.Max >= field);

        static bool InvalidAnyOrder(Ticket ticket, List<Rule> rules, out List<int> InvalidFields) {
            InvalidFields = ticket.Where(field => !rules.Any(rule => Matches(field, rule))).ToList();
            return InvalidFields.Count != 0;
        }


        public override string SolveA(string inputString) {
            var input = ParseInput(inputString);
            var invalid = input.OtherTickets.SelectMany(ticket => {
                if (InvalidAnyOrder(ticket, input.Rules, out var InvalidFields)) {
                    return InvalidFields;
                }
                return new List<int>();
            });
            return invalid.Sum().ToString();
        }

        static bool ValidOrdered(Ticket ticket, List<Rule> rules) {
            return ticket.Zip(rules).All(pair => {
                var field = pair.First;
                var rule = pair.Second;
                var match = Matches(field, rule);
                return match;
            });
        }


        static List<Rule> FindValidOrder(List<Rule> rules, List<Ticket> tickets) {
            var validRules = new List<Rule>();
            var permutations = rules.Permute().ToList();
            foreach (var (permutatiton, idx) in permutations.WithIndex()) {
                System.Console.WriteLine($"checking {idx} of {permutations.Count}");
                if (tickets.All(ticket => ValidOrdered(ticket, permutatiton))) {
                    validRules = permutatiton;
                    break;
                }
            }
            return validRules;
        }

        static List<Ticket> ValidTickets(Input input) =>
            input.OtherTickets.Where(ticket => !InvalidAnyOrder(ticket, input.Rules, out var _)).ToList();

        public override string SolveB(string inputString) {
            var input = ParseInput(inputString);

            var validOrder = FindValidOrder(input.Rules, ValidTickets(input));
            var result =
                validOrder
                    .Select((r, i) => (r, i))
                    .Where(r => r.r.Name.StartsWith("deparature"))
                    .Select(r => input.MyTicket[r.i])
                    .Aggregate(1, (state, n) => state * n);
            return result.ToString();
        }
        public Day16() {
            Tests = new()
            {
                new("sample", "sample", "71", input => GetInput(input).Then(SolveA)),
                new("permute", "sample", "6", input =>
                    GetInput(input)
                    .Then(ParseInput)
                    .Then(input => {
                        return input.Rules.Permute().Count().ToString();
                    })),
                // row class seat
                new("permute", "sample", "1-2-0", input =>
                    GetInput(input)
                    .Then(ParseInput)
                    .Then(input => {
                        var order = FindValidOrder(input.Rules, ValidTickets(input));
                        var @class = order.FindIndex(order => order.Name == "class");
                        var @row = order.FindIndex(order => order.Name == "row");
                        var @seat = order.FindIndex(order => order.Name == "seat");
                        return $"{@class}-{@seat}-{@row}";
                    }))
            };

        }
    }
}
