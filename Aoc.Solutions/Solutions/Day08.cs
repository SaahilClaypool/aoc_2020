using System.Collections.Generic;
using System.Linq;

using Aoc.Runner;

namespace Aoc.Solutions {
    public class Day08 : Day {

        record CState(int Acc, int Ip);
        record Instruction(string Command, int Param);
        static List<Instruction> Parse(string input) =>
            input
                .Split("\n")
                .Select(line => {
                    var parts = line.Split(" ");
                    return new Instruction(parts[0], int.Parse(parts[1].Replace("+", "")));
                })
                .ToList();

        static CState Execute(CState state, Instruction instruction) {
            return instruction switch {
                { Command: "acc" } => state with { Ip = state.Ip + 1, Acc = state.Acc + instruction.Param },
                { Command: "jmp" } => state with { Ip = state.Ip + instruction.Param },
                { Command: "nop" } => state with { Ip = state.Ip + 1 },
                _ => throw new System.NotImplementedException("Unknown command")
            };
        }

        static bool ExecuteToCompletion(List<Instruction> commands, out int result) {
            var visited = new HashSet<int>();
            var state = new CState(0, 0);
            while (!visited.Contains(state.Ip)) {
                result = state.Acc;
                if (state.Ip == commands.Count) {
                    return true;
                }
                else if (state.Ip > commands.Count) {
                    return false;
                }
                visited.Add(state.Ip);
                state = Execute(state, commands[state.Ip]);
            }
            result = state.Acc;
            return false;
        }

        public override string SolveA(string input) {
            var commands = Parse(input);
            ExecuteToCompletion(commands, out var result);
            return result.ToString();
        }

        static IEnumerable<List<Instruction>> PossiblePrograms(List<Instruction> commands) {
            foreach (var (inst, i) in commands.Select((x, i) => (x, i))) {
                if (inst.Command == "nop") {
                    var newCommands = commands.AsEnumerable().ToList();
                    newCommands[i] = inst with { Command = "jmp" };
                    yield return newCommands;
                }
                else if (inst.Command == "jmp") {
                    var newCommands = commands.AsEnumerable().ToList();
                    newCommands[i] = inst with { Command = "nop" };
                    yield return newCommands;
                }
            }
        }

        public override string SolveB(string input) {
            var commands = Parse(input);
            foreach (var program in PossiblePrograms(commands)) {
                if (ExecuteToCompletion(program, out var result)) {
                    return result.ToString();
                }
            }
            return "failed";
        }

        public Day08() {
            Tests = new()
            {
                new("1", "input", "SolveA", (input) => SolveA(input))
            };
        }
    }
}
