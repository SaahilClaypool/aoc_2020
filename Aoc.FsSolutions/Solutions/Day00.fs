module Aoc.Solutions.Day00

open Aoc.Runner
open System.Collections.Generic
open System.Collections

type Day00() =
    inherit Day()

    override this.SolveA(input) = "A"

    override this.SolveB(input) = "B"

    override this.Tests =
        [ Test("fsharp test", "123", "123", (fun input -> "123")) ]
        |> List<Test>
