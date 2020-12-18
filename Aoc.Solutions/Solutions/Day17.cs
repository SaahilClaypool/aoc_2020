using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions {
    public class Day17 : Day {
        class State : Dictionary<Point, Cube> { };
        record Point(long X, long Y, long Z, long W);
        record Cube(Point Location, bool Active) {
            public IEnumerable<Point> Neighbors(bool W = false) {
                for (var x = Location.X - 1; x <= Location.X + 1; x++) {
                    for (var y = Location.Y - 1; y <= Location.Y + 1; y++) {
                        for (var z = Location.Z - 1; z <= Location.Z + 1; z++) {
                            if (W) {
                                for (var w = Location.W - 1; w <= Location.W + 1; w++) {
                                    var point = new Point(x, y, z, w);
                                    if (point != Location)
                                        yield return point;
                                }
                            }
                            else {
                                var point = new Point(x, y, z, 0);
                                if (point != Location)
                                    yield return point;
                            }
                        }
                    }
                }
            }
        }

        static bool ShouldActivate(Cube cube, State World, bool useW) {
            var activeNeighbors = cube.Neighbors(useW).Where(p => World.TryGetValue(p, out var _)).Select(p => World[p]!);
            return (cube.Active, activeNeighbors.Count()) switch {
                (true, 2 or 3) => true,
                (true, _) => false,
                (false, 3) => true,
                _ => false
            };
        }

        static State Cycle(State World, bool useW = false) {
            var nextState = new State();
            foreach (var cube in World.Values) {
                if (ShouldActivate(cube, World, useW)) {
                    nextState[cube.Location] = cube with { Active = true };
                }
                foreach (var adjacent in cube.Neighbors(useW)) {
                    if (!World.TryGetValue(adjacent, out var adjacentCube)) {
                        adjacentCube = new Cube(adjacent, false);
                    }
                    if (ShouldActivate(adjacentCube, World, useW)) {
                        nextState[adjacent] = adjacentCube with { Active = true };
                    }
                }
            }
            return nextState;
        }

        void Show(State world) {
            if (!IsTest) { return; }
            var minX = world.Keys.Select(k => k.X).Min();
            var maxX = world.Keys.Select(k => k.X).Max();
            var minY = world.Keys.Select(k => k.Y).Min();
            var maxY = world.Keys.Select(k => k.Y).Max();
            var minZ = world.Keys.Select(k => k.Z).Min();
            var maxZ = world.Keys.Select(k => k.Z).Max();
            var minW = world.Keys.Select(k => k.W).Min();
            var maxW = world.Keys.Select(k => k.W).Max();

            System.Console.WriteLine($"------------");
            foreach (var w in (minW, maxW).Range(inclusive: true)) {
                System.Console.WriteLine($"W = {w}");
                foreach (var z in (minZ, maxZ).Range(inclusive: true)) {
                    System.Console.WriteLine($"Z = {z}");
                    foreach (var y in (minY, maxY).Range(inclusive: true)) {
                        foreach (var x in (minX, maxX).Range(inclusive: true)) {
                            var l = world.TryGetValue(new(x, y, z, w), out var cube) && cube.Active ? '#' : '.';
                            System.Console.Write(l);
                        }
                        System.Console.Write('\n');
                    }
                }
                System.Console.WriteLine($"------------");
            }
        }

        State ParseInput(string inputString) {
            State world = new();
            foreach (var (line, row) in inputString.Split("\n").WithIndex()) {
                foreach (var (state, column) in line.ToCharArray().WithIndex()) {
                    if (state == '#') {
                        var loc = new Point(column, row, 0, 0);
                        world[loc] = new Cube(loc, state == '#');
                    }
                }
            }
            Show(world);
            return world;
        }


        public override string SolveA(string inputString) {
            var world = ParseInput(inputString);
            foreach (var cycle in Enumerable.Range(1, 6)) {
                Show(world);
                world = Cycle(world);
            }

            Show(world);
            return world.Values.Where(cube => cube.Active).Count().ToString();
        }
        public override string SolveB(string inputString) {
            var world = ParseInput(inputString);
            foreach (var cycle in Enumerable.Range(1, 6)) {
                Show(world);
                world = Cycle(world, true);
            }

            Show(world);
            return world.Values.Where(cube => cube.Active).Count().ToString();
        }

        public Day17() {
            Tests = new()
            {
                new("neighbors", "neighbors", "26", _ => new Cube(new(1, 2, 3, 0), true).Neighbors().Count().ToString()),
                new("sample", "sample", "112", input => GetInput(input).Then(SolveA)),
                new("neighbors4", "neighbors4", "80", _ => new Cube(new(1, 2, 3, 0), true).Neighbors(true).Count().ToString()),
                new("sample4", "sample", "848", input => GetInput(input).Then(SolveB))
            };
        }
    }
}
