module Aoc.Solutions.Day02

open Aoc.Runner
open System.Collections.Generic
open System.Collections

type Line =
    { Min: int
      Max: int
      L: char
      Password: string }


let ParseLine (line: string) =
    let p = line.Split(" ")
    let a, b, c = (p.[0], p.[1], p.[2])

    { Min = int (a.Split("-").[0])
      Max = int (a.Split("-").[1])
      L = b.[0]
      Password = c }

let IsValidA (L: Line) =
    let v =
        L.Password
        |> Seq.filter (fun x -> x = L.L)
        |> Seq.length

    v >= L.Min && v <= L.Max

let IsValidB (L: Line) =
    (L.Password.[L.Min - 1] = L.L)
    <> (L.Password.[L.Max - 1] = L.L)


type Day02() =
    inherit Day()

    override this.SolveA(input) =
        input.Split("\n")
        |> Seq.map (ParseLine)
        |> Seq.filter (IsValidA)
        |> Seq.length
        |> string

    override this.SolveB(input) =
        input.Split("\n")
        |> Seq.map (ParseLine)
        |> Seq.filter (IsValidB)
        |> Seq.length
        |> string

    override this.Tests =
        [ Test("fsharp test", "123", "123", (fun input -> "123")) ]
        |> List<Test>
