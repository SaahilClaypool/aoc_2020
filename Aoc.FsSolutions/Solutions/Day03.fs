module Aoc.Solutions.Day03

open Aoc.Runner
open System.Collections.Generic
open System.Collections


let solveWithSlope (input: string, dx, dy) =
    let lines = input.Split("\n")

    let maxLength =
        (lines |> Seq.maxBy (fun l -> l.Length)).Length

    let nextLine (x, total) (line: string) =
        let newTotal =
            match line.[x] with
            | '#' -> total + 1
            | _ -> total

        (((x + dx) % maxLength), newTotal)

    let result =
        lines
        |> Seq.indexed
        |> Seq.filter (fun (i, s) -> i % dy = 0)
        |> Seq.map (fun (i, s) -> s)
        |> Seq.fold nextLine (0, 0)

    let (_, total) = result
    total



type Day03() =
    inherit Day()

    override this.SolveA(input) = solveWithSlope (input, 3, 1) |> string

    override this.SolveB(input) =
        [| 1, 1; 3, 1; 5, 1; 7, 1; 1, 2 |]
        |> Seq.map (fun (dx, dy) -> solveWithSlope (input, dx, dy) |> int64)
        |> Seq.reduce ((*))
        |> string

    override this.Tests =
        [ Test("fsharp test", "123", "123", (fun input -> "123")) ]
        |> List<Test>
