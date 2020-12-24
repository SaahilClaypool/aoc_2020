using System.Collections.Generic;
using System.Linq;

using Aoc.Runner;

using Extensions;

namespace Aoc.Solutions.D20 {

    record Loc(int Row, int Col);

    class Tile {
        public long Label { get; init; }
        public int Rotation = 0;
        public List<List<char>> BaseContent { get; init; }
        public int Cols => BaseContent[0].Count;
        public int Rows => BaseContent.Count;
        public int RotatedCols => Content()[0].Count;
        public int RotatedRows => Content().Count;
        int MaxCol => Cols - 1;
        int MaxRow => Rows - 1;
        int MaxColR => RotatedCols - 1;
        int MaxRowR => RotatedRows - 1;

        public Loc Loc = new(0, 0);

        Loc Index(int row, int col) => Index(row, col, Rotation);
        Loc Index(int row, int col, int rotation) {
            return rotation switch {
                0 => new(row, col), // up
                1 => new(col, MaxRow - row), // right
                2 => new(MaxRow - row, MaxCol - col), // down
                3 => new(MaxCol - col, row), // left
                4 or 5 or 6 or 7 => Index(row, MaxCol - col, Rotation % 4), // flip the peice then rotate
                _ => throw new System.Exception("Invalid rotation")
            };
        }

        private readonly Dictionary<int, List<List<char>>> ContentCache = new();
        public List<List<char>> Content() {
            if (ContentCache.ContainsKey(Rotation)) {
                return ContentCache[Rotation];
            }
            // var view = BaseContent.Select(row => new List<char>(row)).ToList();
            var dimmensions = new[] {
                Index(0, 0),
                Index(MaxRow, MaxCol)
            };
            var maxRow = dimmensions.Select(d => d.Row).Max();
            var maxCol = dimmensions.Select(d => d.Col).Max();

            var view = Enumerable.Range(0, maxRow + 1).Select(_ =>
                Enumerable.Range(0, maxCol + 1).Select(_ => ' ').ToList()).ToList();
            foreach (var (row, r) in BaseContent.WithIndex()) {
                foreach (var (col, c) in row.WithIndex()) {
                    var (newRow, newCol) = Index(r, c);
                    view[newRow][newCol] = col;
                }
            }
            ContentCache[Rotation] = view;
            return view;
        }

        public List<char> Top() => Content()[0];
        public List<char> Bottom() => Content()[MaxRow];
        public List<char> Left() => Content().Select(r => r[0]).ToList();
        public List<char> Right() => Content().Select(r => r[MaxCol]).ToList();

        // mutates self
        public bool FitAround(Tile other) {
            // for each rotation, 
            foreach (var i in Enumerable.Range(0, 8)) {
                Rotation = i;
                if (SideEqual(Bottom(), other.Top())) {
                    Loc = new(other.Loc.Row - 1, other.Loc.Col);
                    return true;
                }
                if (SideEqual(Right(), other.Left())) {
                    Loc = new(other.Loc.Row, other.Loc.Col - 1);
                    return true;
                }
                if (SideEqual(Left(), other.Right())) {
                    Loc = new(other.Loc.Row, other.Loc.Col + 1);
                    return true;
                }
                if (SideEqual(Top(), other.Bottom())) {
                    Loc = new(other.Loc.Row + 1, other.Loc.Col);
                    return true;
                }

            }
            return false;
        }

        public void PlaceInPuzzle(Tile puzzle, int minRow, int minCol) {
            var rowOffset = (Loc.Row - minRow) * (Rows - 2);
            var colOffset = (Loc.Col - minCol) * (Cols - 2);

            foreach (var (row, r) in Content().WithIndex()) {
                foreach (var (col, c) in row.WithIndex()) {
                    if (c == 0 || c == MaxCol || r == 0 || r == MaxRow) {
                        continue;
                    }
                    var puzzleRow = rowOffset + r;
                    var puzzleCol = colOffset + c;
                    puzzle.BaseContent[puzzleRow][puzzleCol] = col;
                }
            }
        }

        // search in basecontent to avoid rotation
        public bool PatternAtLocation(Loc loc, Tile pattern) {
            var patternHashesToFind = pattern.Content().Select(row => row.Where(c => c == '#').Count()).Sum();
            foreach (var r in Enumerable.Range(0, pattern.RotatedRows)) {
                foreach (var c in Enumerable.Range(0, pattern.RotatedCols)) {
                    var patternChar = pattern.Content()[r][c];
                    if (patternChar != '#') {
                        continue;
                    }
                    var pr = loc.Row + r;
                    var pc = loc.Col + c;
                    if (pr > MaxRowR || pc > MaxColR) {
                        continue;
                    }
                    if (Content()[pr][pc] != '#') {
                        return false;
                    }
                    patternHashesToFind -= 1;
                }
            }

            var found = patternHashesToFind == 0;

            if (found) {
                foreach (var r in Enumerable.Range(0, pattern.RotatedRows)) {
                    foreach (var c in Enumerable.Range(0, pattern.RotatedCols)) {
                        var patternChar = pattern.Content()[r][c];
                        if (patternChar != '#') {
                            continue;
                        }
                        var pr = loc.Row + r;
                        var pc = loc.Col + c;
                        if (pr > MaxRowR || pc > MaxColR) {
                            continue;
                        }
                        Content()[pr][pc] = 'O';
                    }
                }
            }

            return found;
        }

        public static bool SideEqual(List<char> a, List<char> b) =>
            a.Count == b.Count && a.Zip(b).All(pair => pair.First == pair.Second);
    }

    public class Day20 : Day {

        void ShowPuzzle(Dictionary<Loc, Tile> puzzle) {
            var minCol = puzzle.Keys.Min(l => l.Col);
            var maxCol = puzzle.Keys.Max(l => l.Col);
            var minRow = puzzle.Keys.Min(l => l.Row);
            var maxRow = puzzle.Keys.Max(l => l.Row);

            foreach (var r in Enumerable.Range(minRow, maxRow - minRow + 1)) {
                foreach (var c in Enumerable.Range(minCol, maxCol - minCol + 1)) {
                    System.Console.Write($" {puzzle[new(r, c)].Label} ");
                }
                System.Console.WriteLine();
            }
        }


        Dictionary<Loc, Tile> AssemblePuzzle(List<Tile> tiles) {
            var ogCount = tiles.Count;
            tiles = new List<Tile>(tiles); // copy
            var puzzle = new Dictionary<Loc, Tile>();
            var first = tiles.First();
            tiles.Remove(first);
            puzzle.Add(new(0, 0), first);

            while (tiles.Count > 0) {
                foreach (var tile in tiles.Where(t => !puzzle.ContainsValue(t))) {
                    if (puzzle.Values.Any(placed => tile.FitAround(placed!))) {
                        puzzle.Add(tile.Loc, tile);
                        tiles.Remove(tile);
                        $"Placed {puzzle.Values.Count} of {ogCount}".Dbg();
                        break;
                    }
                }
            }
            return puzzle;
        }

        public override string SolveA(string input) {
            var tiles = Parse(input);
            var puzzle = AssemblePuzzle(tiles);
            var minCol = puzzle.Keys.Min(l => l.Col);
            var maxCol = puzzle.Keys.Max(l => l.Col);
            var minRow = puzzle.Keys.Min(l => l.Row);
            var maxRow = puzzle.Keys.Max(l => l.Row);

            return (puzzle[new(minRow, minCol)].Label *
                    puzzle[new(maxRow, minCol)].Label *
                    puzzle[new(maxRow, maxCol)].Label *
                    puzzle[new(minRow, maxCol)].Label).ToString();
        }

        public override string SolveB(string input) {
            var tiles = Parse(input);
            var puzzle = AssemblePuzzle(tiles);
            var minCol = puzzle.Keys.Min(l => l.Col);
            var maxCol = puzzle.Keys.Max(l => l.Col);
            var minRow = puzzle.Keys.Min(l => l.Row);
            var maxRow = puzzle.Keys.Max(l => l.Row);
            var tileSize = tiles[0].BaseContent.Count;

            ShowPuzzle(puzzle);

            var puzzleTile = new Tile() {
                BaseContent = Enumerable.Range(0, (maxRow - minRow + 1) * tileSize).Select(
                    _ => Enumerable.Range(0, (maxCol - minCol + 1) * tileSize).Select(_ => 'x').ToList()
                ).ToList()
            };
            foreach (var tile in puzzle.Values) {
                tile.PlaceInPuzzle(puzzleTile, minRow, minCol);
            }


            System.Console.WriteLine("\nPUZZLE:\n");
            foreach (var row in puzzleTile.Content()) {
                System.Console.WriteLine(new string(row.ToArray()));
            }

            var dragonTile = ParseTile(GetInput("dragon"));

            var found = 0;
            foreach (var rotation in Enumerable.Range(0, 8)) {
                puzzleTile.Rotation = rotation;
                System.Console.WriteLine("\nPUZZLE:\n");
                foreach (var row in puzzleTile.Content()) {
                    System.Console.WriteLine(new string(row.ToArray()));
                }
                foreach (var row in Enumerable.Range(0, puzzleTile.BaseContent.Count)) {
                    foreach (var col in Enumerable.Range(0, puzzleTile.BaseContent[0].Count)) {
                        if (puzzleTile.PatternAtLocation(new(row, col), dragonTile)) {
                            found += 1;
                        }
                    }
                }

                if (found > 0) {
                    System.Console.WriteLine($"Found {found}");
                    System.Console.WriteLine("\nPUZZLE:\n");
                    foreach (var row in puzzleTile.Content()) {
                        System.Console.WriteLine(new string(row.ToArray()));
                    }
                    return puzzleTile.Content().Select(
                        row => row.Where(c => c == '#').Count()
                    ).Sum().ToString();
                }
            }
            return "B";
        }


        Tile ParseTile(string block) {
            var lines = block.Split("\n");
            var label = long.Parse(lines[0].Split(" ")[1].Replace(":", ""));
            var content = lines.Skip(1).Select(l => l.ToCharArray().ToList()).ToList();
            return new Tile { Label = label, BaseContent = content };
        }
        List<Tile> Parse(string input) =>
            input.Split("\n\n")
                .Select(ParseTile)
                .ToList();


        public Day20() {
            Tests = new()
            {
                new("sample A", "sample", "20899048083289", input => GetInput(input).Then(SolveA)),
                new("sample B", "sample", "273", input => GetInput(input).Then(SolveB))
            };
        }
    }
}
