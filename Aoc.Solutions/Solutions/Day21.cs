using System.Collections.Generic;
using System.Linq;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions.D21 {

    class Solve {
        public Dictionary<string, HashSet<string>> Foods { get; init; } = new();
        public List<(HashSet<string> foods, HashSet<string> allergens)> Rules { get; init; } = new();
        public HashSet<string> Allergens { get; set; } = new();
        public HashSet<string> Ingredients { get; set; } = new();

        public Dictionary<string, HashSet<string>> IngredientsByAllergen() =>
            Allergens.ToDictionary(
                allergen => allergen,
                allergen => Rules.Where(entry => entry.allergens.Contains(allergen))
                    .Aggregate(
                        Ingredients as IEnumerable<string>,
                        // for every rule that has this allergen,
                        // the possible foods are the intersection of all the foods that could have made this allergen
                        // Because, every time the allergen is seen, the same food must be on the left side
                        (acc, entry) => acc.Intersect(entry.foods)
                    ).ToHashSet()
                );

        void Setup() {
            Allergens = new HashSet<string>();
            Ingredients = new HashSet<string>();
            foreach (var (foodRule, allergenRule) in Rules) {
                Allergens.UnionWith(allergenRule);
                Ingredients.UnionWith(foodRule);
            }
        }

        public string SolveA() {
            Setup();
            var suspiciousIngredients = IngredientsByAllergen()
                .SelectMany(kvp => kvp.Value)
                .ToHashSet();
            suspiciousIngredients.Dbg();
            var nonSuspiciousIngredients = Ingredients.Except(suspiciousIngredients);
            nonSuspiciousIngredients.Dbg();
            return Rules.Select(entry => entry.foods.Count(ingredient => !suspiciousIngredients.Contains(ingredient))).Sum()
            .ToString();
        }

        public string SolveB() {
            Setup();
            var suspiciousIngredients = IngredientsByAllergen();

            var solved = new Dictionary<string, string>();

            while (suspiciousIngredients.Any(pair => pair.Value.Count > 0)) {
                var solveable = suspiciousIngredients.First(pair => pair.Value.Count == 1);
                solved.Add(solveable.Key, solveable.Value.First());
                foreach (var kvp in suspiciousIngredients) {
                    kvp.Value.Remove(solved[solveable.Key]);
                }
            }

            // invert lookup to be by the ingredient to allergen
            solved = solved.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            solved.Dbg();
            return string.Join(",", solved.Keys.OrderBy(ingredient => solved[ingredient]));
        }
    }

    public class Day21 : Day {

        static (List<(HashSet<string> foods, HashSet<string> allergens)> rules, Dictionary<string, HashSet<string>> foods) Parse(string content) {
            static (HashSet<string> foods, HashSet<string> allergens) ParseLine(string line) {
                var parts = line.Split("(contains");
                var foods = parts[0].Trim().Split(" ").Select(w => w.Trim());
                var allergens = parts[1].Trim().Split(",").Select(w => w.Trim().Replace(")", ""));

                return (new(foods), new(allergens));
            }

            var lines = content
                .Split("\n")
                .Select(ParseLine)
                .ToList();

            Dictionary<string, HashSet<string>> possibleAllergens = new();

            foreach (var (foods, allergens) in lines) {
                foreach (var food in foods) {
                    if (!possibleAllergens.ContainsKey(food)) {
                        possibleAllergens.Add(food, new());
                    }
                    possibleAllergens[food].UnionWith(allergens);
                }
            }

            return (lines, possibleAllergens);
        }

        public override string SolveA(string input) {
            var (rules, foods) = Parse(input);
            Solve s = new() { Foods = foods, Rules = rules };

            return s.SolveA();
        }

        public override string SolveB(string input) {
            var (rules, foods) = Parse(input);
            Solve s = new() { Foods = foods, Rules = rules };

            return s.SolveB();
        }

        public Day21() {
            Tests = new()
            {
                new("sample", "sample", "5", input => GetInput(input).Then(SolveA)),
                new("sample", "sample", "mxmxvkd,sqjhc,fvjkl", input => GetInput(input).Then(SolveB))
            };

        }
    }
}
