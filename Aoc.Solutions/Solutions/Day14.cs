using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions {
    public class Day14 : Day {
        record Command() {
            public record Set(ulong Address, ulong Value) : Command;
            public record Mask(string Value) : Command;
        }
        record Input(List<Command> Commands);
        static Input ParseInput(string input) {
            static Command ParseLine(string line) {
                var set = new Regex(@"mem\[(?<address>\d+)\] = (?<value>\d+)").Match(line);
                if (set.Success) {
                    return new Command.Set(
                        ulong.Parse(set.Groups["address"].Value),
                        ulong.Parse(set.Groups["value"].Value)
                    );
                }
                else {
                    return new Command.Mask(line.Split(" = ")[1]);
                }
            }
            var commands = input.Split("\n")
                .Select(ParseLine).ToList();
            return new(commands);
        }
        public override string SolveA(string input) {
            var setup = ParseInput(input);
            var state = new Dictionary<ulong, ulong>();
            var mask = "";

            ulong ApplyMask(ulong input, string mask) {
                foreach (var (letter, power) in mask.Reverse().WithIndex()) {
                    var bitmask = 1UL << power;
                    input = letter switch {
                        '0' => input & ~bitmask,
                        '1' => input | bitmask,
                        _ => input
                    };
                }
                return input;
            }

            foreach (var command in setup.Commands) {
                if (command is Command.Set(var address, var value)) {
                    state[address] = ApplyMask(value, mask);
                }
                else if (command is Command.Mask(var newMask)) {
                    mask = newMask;
                }
                else throw new Exception("cannot parse command");
            }

            return state.Values.Aggregate((a, b) => a + b).ToString(); ;
        }

        public override string SolveB(string input) {
            var setup = ParseInput(input);
            var state = new Dictionary<ulong, ulong>();
            var mask = "";

            static IEnumerable<ulong> ApplyMask(ulong address, string mask) {
                var xBitmasks = new List<ulong>();
                foreach (var (letter, power) in mask.Reverse().WithIndex()) {
                    var bitmask = 1UL << power;
                    if (letter == 'X') {
                        xBitmasks.Add(bitmask);
                    }
                    address = letter switch {
                        '0' => address,
                        '1' => address | bitmask,
                        _ => address
                    };
                }

                foreach (var combination in xBitmasks.Combinations()) {
                    var tempAddress = address;
                    foreach (var bitmask in xBitmasks) {
                        if (combination.Contains(bitmask)) {
                            tempAddress |= bitmask;
                        }
                        else {
                            tempAddress &= ~bitmask;
                        }
                    }
                    yield return tempAddress;
                }
            }

            foreach (var command in setup.Commands) {
                if (command is Command.Set(var address, var value)) {
                    foreach (var newAddress in ApplyMask(address, mask).ToList()) {
                        state[newAddress] = value;
                    }
                }
                else if (command is Command.Mask(var newMask)) {
                    mask = newMask;
                }
                else throw new Exception("cannot parse command");
            }

            return state.Values.Aggregate((a, b) => a + b).ToString(); ;
        }

        static string Bin(ulong bitmask) => Convert.ToString((long)bitmask, 2);

        public Day14() {
            Tests = new()
            {
                new("A", "sample", "165", input => GetInput(input).Then(SolveA)),
                new("B", "sampleb", "208", input => GetInput(input).Then(SolveB))
            };

        }
    }
}
