using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions {
    public class Day07 : Day {
        record RuleItem(string Color, int Count);
        record Rule(string Color, IEnumerable<RuleItem> Holds);
        static Rule ParseLine(string line) {
            var color = string.Join(" ", line.Split(" ").Take(2));
            var rules = line.Split("contain").ToList()[1].Split(",");
            var ruleExp = new Regex(@"(?:.*)(?<count>\d+) (?<rulecolor>.+) bag(?:.*)");
            var ruleTuples = new List<RuleItem>();
            try {
                ruleTuples = rules.Select(rule => {
                    var groups = ruleExp.Match(rule).Groups;
                    System.Console.WriteLine(rule);
                    var result = new RuleItem(Color: groups["rulecolor"].Value, Count: int.Parse(groups["count"].Value));
                    System.Console.WriteLine(result);
                    return result;
                }).ToList();
            }
            catch { }
            return new Rule(color, ruleTuples);
        }

        static Dictionary<string, IEnumerable<RuleItem>> ToTree(IEnumerable<Rule> rules) {
            return rules.ToDictionary(item => item.Color, item => item.Holds);
        }

        static Dictionary<string, HashSet<string>> InvertTree(Dictionary<string, IEnumerable<RuleItem>> tree) {
            var newTree = new Dictionary<string, HashSet<string>>();
            foreach (var (Key, Value) in tree) {
                foreach (var item in Value) {
                    if (newTree.TryGetValue(item.Color, out var items)) {
                        items.Add(Key);
                    }
                    else {
                        newTree[item.Color] = new HashSet<string> {
                            Key
                        };
                    }
                }
            }
            return newTree;
        }

        public override string SolveA(string input) {
            var rules = ToTree(input.Split("\n").Select(ParseLine));
            var inverted = InvertTree(rules);
            var searched = new HashSet<string>();
            var active = new HashSet<string>() { "shiny gold" };
            while (active.Any()) {
                var node = active.First();
                active.Remove(node);
                searched.Add(node);
                if (inverted.ContainsKey(node))
                    foreach (var parent in inverted[node].Where(node => !searched.Contains(node)))
                        active.Add(parent);
            }

            return (searched.Count - 1).ToString();
        }

        static int CountForRule(Dictionary<string, IEnumerable<RuleItem>> rules, string color) {
            return 1 + rules[color].Select(rule => CountForRule(rules, rule.Color) * rule.Count).Sum();
        }

        public override string SolveB(string input) {
            var rules = ToTree(input.Split("\n").Select(ParseLine));
            return (CountForRule(rules, "shiny gold") - 1).ToString();
        }
        public Day07() {
            Tests = new()
            {
                new("1", "sample", "4", input => GetInput(input).Then(SolveA)),
                new("2", "sample", "32", input => GetInput(input).Then(SolveB)),
                new("3", "sample2", "126", input => GetInput(input).Then(SolveB))
            };

        }
    }
}
