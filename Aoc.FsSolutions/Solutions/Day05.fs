module Aoc.Solutions.Day05

open Aoc.Runner
open System.Collections.Generic
open System.Collections

type SearchState = { Min: int; Max: int }
type State = { Row: SearchState; Col: SearchState }

let midpoint s = (s.Min + s.Max) / 2

let upper (s: SearchState) = { Min = (midpoint s + 1); Max = s.Max }

let lower (s: SearchState) = { Min = s.Min; Max = midpoint s }



let search st =
    let state =
        st
        |> Seq.fold (fun state c ->
            match c with
            | 'F' ->
                { Row = lower state.Row
                  Col = state.Col }
            | 'B' ->
                { Row = upper state.Row
                  Col = state.Col }
            | 'R' ->
                { Row = state.Row
                  Col = upper state.Col }
            | 'L' ->
                { Row = state.Row
                  Col = lower state.Col }
            | _ -> state)
               { Row = { Min = 0; Max = 127 }
                 Col = { Min = 0; Max = 7 } }

    (midpoint state.Row, midpoint state.Col)

let sid (row, col) = row * 8 + col

type Day05() =
    inherit Day()

    override this.SolveA(input) =
        let seats = input.Split("\n") |> Seq.map search

        // for inp in input.Split("\n") do
        //     let seat = search inp
        //     printfn $"Seat: {inp} {seat} {id seat}"

        seats |> Seq.map sid |> Seq.max |> string

    override this.SolveB(input) =
        let seats =
            input.Split("\n") |> Seq.map search |> HashSet

        let ids =
            (Seq.map (search >> sid) (input.Split("\n")))
            |> HashSet

        let allseats =
            seq {
                for r in 0 .. 127 do
                    for c in 0 .. 7 -> (r, c)
            }

        (allseats
        |> Seq.find (fun (r, c) ->
            not (seats.Contains((r, c)))
            && ids.Contains(sid (r, c) - 1)
            && ids.Contains(sid (r, c) + 1)))
        |> sid
        |> string



    override this.Tests =
        [ Test("1", "BFFFBBFRRR", "(70, 7)", (fun input -> (search >> string) input))
          Test("2", "FFFBBBFRRR", "(14, 7)", (fun input -> (search >> string)  input))
          Test("3", "BBFFBBFRLL", "(102, 4)", (fun input -> (search >> string)  input))
          Test("4", "BFFFBBFRRR", "567", (fun input -> (search >> sid >> string)  input))
          Test("5", "FFFBBBFRRR", "119", (fun input -> (search >> sid >> string)  input))
          Test("7", "FBFBBFFRLR", "357", (fun input -> (search >> sid >> string)  input))
          Test("6", "BBFFBBFRLL", "820", (fun input -> (search >> sid >> string)  input)) ]

        |> List<Test>
