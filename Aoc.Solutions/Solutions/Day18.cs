using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions {
    public class Day18 : Day {
        private static bool IsTest = false;

        class Value {
            [JsonIgnore]
            public Operation? Parent { get; set; }
            public virtual long Val { get; init; } = 0;
            public virtual void Show() {
                if (!IsTest) return;
                var buffer = "\n--";
                Show(ref buffer);
                System.Console.WriteLine(buffer);
            }
            public virtual void Show(ref string buffer, int depth = 0) {
                if (!IsTest) { return; }
                buffer += "\n";
                foreach (var i in Enumerable.Range(0, depth)) buffer += "..";
                buffer += Val;
            }
        };
        class Unset : Value {
            public override long Val { get; init; } = -1;
            public override void Show(ref string buffer, int depth = 0) {
                if (!IsTest) { return; }
                buffer += "\n";
                foreach (var i in Enumerable.Range(0, depth)) buffer += "..";
                buffer += "Unset";
            }
        };
        class Operation : Value {
            public enum OpType { Add, Mult }
            public Value Left { get; set; } = new Unset();
            public Value Right { get; set; } = new Unset();
            public OpType Op { get; set; }
            public override long Val => Op switch {
                OpType.Add => Left.Val + Right.Val,
                OpType.Mult => Left.Val * Right.Val,
                var v => throw new System.Exception($"not implemented operation: {v}")
            };
            public override void Show(ref string buffer, int depth = 0) {
                if (!IsTest) { return; }
                buffer += "\n";
                foreach (var i in Enumerable.Range(0, depth)) buffer += "..";
                buffer += Op.ToJson();
                Left.Show(ref buffer, depth + 1);
                Right.Show(ref buffer, depth + 1);
            }
        }

        Operation Parse(List<string> words, out int consumed) {
            var CurrentOp = new Operation();
            for (var i = 0; i < words.Count; i++) {
                var word = words[i];
                if (word == "+") {
                    if (CurrentOp.Right is Unset) {
                        CurrentOp.Op = Operation.OpType.Add;
                    }
                    else {
                        CurrentOp = new Operation { Left = CurrentOp, Op = Operation.OpType.Add };
                    }
                }
                else if (word == "*") {
                    if (CurrentOp.Right is Unset) {
                        CurrentOp.Op = Operation.OpType.Mult;
                    }
                    else {
                        CurrentOp = new Operation { Left = CurrentOp, Op = Operation.OpType.Mult };
                    }
                }
                else if (word.StartsWith("(")) {
                    var subtree = Parse(words.Skip(i + 1).ToList(), out var nextConsumed);
                    if (CurrentOp.Left is Unset) {
                        CurrentOp.Left = subtree;
                    }
                    else {
                        CurrentOp.Right = subtree;
                    }
                    i += nextConsumed;
                }
                else if (word.EndsWith(")")) {
                    consumed = i + 1;
                    return CurrentOp;
                }
                else {
                    if (CurrentOp.Left is Unset) {
                        CurrentOp.Left = new Value { Val = long.Parse(word) };
                    }
                    else {
                        CurrentOp.Right = new Value { Val = long.Parse(word) };
                    }
                }
            }

            consumed = words.Count;
            return CurrentOp;
        }

        /// make parenthesis their own "word" for easier parsing
        public static string PreFormat(string inputString) => inputString.Replace("(", "( ").Replace(")", " )");

        public long SolveProblem(string inputString) {
            var op = Parse(PreFormat(inputString).Split().ToList(), out var _);
            var output = "";
            op.Show(ref output);
            System.Console.Write(output);
            return op.Val;
        }

        public override string SolveA(string inputString) {
            IsTest = base.IsTest;
            var result = inputString.Split("\n").Select(SolveProblem).Sum().ToString();
            return result.ToString();
        }

        public long SolveProblemB(string inputString) {
            IsTest = base.IsTest;
            var op = ParseTree(PreFormat(inputString).Split().ToList(), out var _);
            return op.Val;
        }

        Operation ParseTree(List<string> words, out int consumed) {
            var CurrentOp = new Operation();
            Operation Root() {
                var current = CurrentOp;
                while (current?.Parent is not null) current = current.Parent;
                return current;
            }
            for (var i = 0; i < words.Count; i++) {
                Root().Show();
                var word = words[i];
                if (word == "+") {
                    if (CurrentOp.Right is Unset) {
                        CurrentOp.Op = Operation.OpType.Add;
                    }
                    // rotate tree so this happens before whatever was above it
                    else {
                        var parentRight = CurrentOp.Right;
                        var newSubtree = new Operation { Left = parentRight, Op = Operation.OpType.Add, Parent = CurrentOp };
                        parentRight.Parent = newSubtree;
                        CurrentOp.Right = newSubtree;
                        CurrentOp = newSubtree;
                    }
                }
                else if (word == "*") {
                    if (CurrentOp.Right is Unset) {
                        CurrentOp.Op = Operation.OpType.Mult;
                    }
                    else {
                        // happens after whatever else comes next
                        // first parent that is not addition
                        var parent = CurrentOp;
                        while (parent is not null && parent.Op == Operation.OpType.Add && parent.Parent is not null) parent = parent.Parent;
                        var current = CurrentOp;
                        CurrentOp = new Operation { Left = parent!, Op = Operation.OpType.Mult, Parent = parent!.Parent };
                        if (parent!.Parent?.Left == current) {
                            parent.Left = CurrentOp;
                        }
                        else if (parent!.Parent?.Right == current) {
                            parent.Right = CurrentOp;
                        }
                    }
                }
                else if (word.StartsWith("(")) {
                    var subtree = ParseTree(words.Skip(i + 1).ToList(), out var nextConsumed);
                    if (CurrentOp.Left is Unset) {
                        CurrentOp.Left = subtree;
                    }
                    else {
                        CurrentOp.Right = subtree;
                    }
                    i += nextConsumed;
                }
                else if (word.EndsWith(")")) {
                    consumed = i + 1;
                    return Root();
                }
                else {
                    if (CurrentOp.Left is Unset) {
                        CurrentOp.Left = new Value { Val = long.Parse(word) };
                    }
                    else {
                        CurrentOp.Right = new Value { Val = long.Parse(word) };
                    }
                }
            }

            consumed = words.Count;
            return Root();
        }

        public override string SolveB(string inputString) {
            IsTest = base.IsTest;
            var result = inputString.Split("\n").Select(SolveProblemB).Sum().ToString();
            return result.ToString();
        }

        public Day18() {
            Tests = new() {
                new("sample", "1 + 2 * 3 + 4 * 5 + 6", "71", input => SolveA(input)),
                new("parenthesis", "5 + (8 * 3 + 9 + 3 * 4 * 3)", "437", input => SolveProblem(input).ToString()),
                new("parenthesis", "5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", "12240", input => SolveProblem(input).ToString()),
                new("parenthesis 2 bugaloo", "((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", "13632", input => SolveProblem(input).ToString().ToString()),
                new("parenthesis w/ pref", "1 + (2 * 3) + (4 * (5 + 6))", "51", input => SolveProblemB(input).ToString()),
                new("simple pref", "2 * 3 + (4 * 5)", "46", input => SolveProblemB(input).ToString()),
                new("pref", "5 + (8 * 3 + 9 + 3 * 4 * 3)", "1445", input => SolveProblemB(input).ToString()),
                new("last", "((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", "23340", input => SolveProblemB(input).ToString()),
            };
        }
    }
}
