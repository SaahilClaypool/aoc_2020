module Aoc.Solutions

open Aoc.Runner
open System.Collections

let triples (enumerable: int []) =
    seq {
        for i in 0 .. (enumerable.Length - 3) do
            for j in i + 1 .. (enumerable.Length - 2) do
                for k in j + 1 .. (enumerable.Length - 1) -> (enumerable.[i], enumerable.[j], enumerable.[k])
    }

let doubles (enumerable: int []) =
    seq {
        for i in 0 .. (enumerable.Length - 2) do
            for j in i + 1 .. (enumerable.Length - 1) -> (enumerable.[i], enumerable.[j])
    }


type Day01() =
    inherit Day()

    override this.SolveA(input) =
        let lines = input.Split("\n") |> Array.map int

        let pairs = lines |> doubles

        let solution =
            pairs
            |> Seq.filter (fun (a, b) -> a + b = 2020)
            |> Seq.head

        let (a, b) = solution
        (a * b).ToString()

    override this.SolveB(input) =
        let solution =
            input.Split("\n")
            |> Array.map int
            |> triples
            |> Seq.filter (fun (a, b, c) -> a + b + c = 2020)
            |> Seq.head

        let (a, b, c) = solution
        (a * b * c).ToString()
