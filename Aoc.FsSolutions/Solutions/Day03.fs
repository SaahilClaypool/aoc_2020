module Aoc.Solutions.Day03

open Aoc.Runner
open System.Collections.Generic
open System.Collections


let ParseLine (line: string) = line


type Day03() =
    inherit Day()

    override this.SolveA(input) = "A"

    override this.SolveB(input) = "B"

    override this.Tests =
        [ Test("fsharp test", "123", "123", (fun input -> "123")) ]
        |> List<Test>
