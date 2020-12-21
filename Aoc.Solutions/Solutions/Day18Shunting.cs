using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions {
    public class Day18Shunting : Day {
        Dictionary<string, int> DefaultPrecedence = new() {
            ["+"] = 1,
            ["-"] = 1,
            ["("] = 10,
        };
        Dictionary<string, int> PlusPrecendence = new() {
            ["+"] = 2,
            ["-"] = 1,
            ["("] = 10,
        };

        List<string> ToRps(string input, Dictionary<string, int> precedence) {
            Stack<string> operators = new();
            List<string> output = new();
            foreach (var word in input.Split()) {
                if (word == "+" || word == "-" || word == "(") {
                    while (
                        operators.TryPeek(out var top) &&
                        precedence[top] > precedence[word]
                    ) {
                        output.Add(operators.Pop());
                    }
                    operators.Push(word);
                }
                else if (word == ")") {
                    while (
                        operators.TryPeek(out var top) &&
                        top != "("
                    ) {
                        output.Add(operators.Pop());
                    }
                    operators.Pop();
                }
                else {
                    output.Add(word);
                }
            }
            while (operators.TryPop(out var val)) output.Add(val);

            output.Dbg();

            return output;
        }

        public override string SolveA(string input) {
            return ToRps(Day18.PreFormat(input), DefaultPrecedence).ToString();
        }


        public Day18Shunting() {
            Tests = new() {
                new("sample", "1 + 2 * 3 + 4 * 5 + 6", "71", input => SolveA(input))
            };
        }
    }
}
