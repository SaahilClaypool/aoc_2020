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
            var ruleExp = new Regex(@"(?<count>\d+) (?<rulecolor>[a-z]+ [a-z]+) bag");
            var ruleTuples = ruleExp
                .Matches(line)
                .Select(m => new RuleItem(m.Groups["rulecolor"].Value, int.Parse(m.Groups["count"].Value)))
                .ToList();
            return new Rule(color, ruleTuples);
        }

        static Dictionary<string, IEnumerable<RuleItem>> ToTree(IEnumerable<Rule> rules) {
            return rules.ToDictionary(item => item.Color, item => item.Holds);
        }

        static Dictionary<string, HashSet<string>> InvertTree(Dictionary<string, IEnumerable<RuleItem>> tree) {
            var parentTree = new Dictionary<string, HashSet<string>>();
            foreach (var (Key, Value) in tree) {
                foreach (var item in Value) {
                    if (parentTree.TryGetValue(item.Color, out var items)) {
                        items.Add(Key);
                    }
                    else {
                        parentTree[item.Color] = new HashSet<string> {
                            Key
                        };
                    }
                }
            }
            return parentTree;
        }

        public override string SolveA(string input) {
            var rules = ToTree(input.Split("\n").Select(ParseLine));
            var parents = InvertTree(rules);
            IEnumerable<string> parentsOf(string bag) {
                if (parents.ContainsKey(bag)) {
                    foreach (var parent in parents[bag]) {
                        yield return parent;
                        foreach (var parentParent in parentsOf(parent)) {
                            yield return parentParent;
                        }
                    }
                }
            }
            return parentsOf("shiny gold").ToHashSet().Count.ToString();
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
